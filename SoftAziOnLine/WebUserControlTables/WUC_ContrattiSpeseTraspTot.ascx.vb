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

Partial Public Class WUC_ContrattiSpeseTraspTot
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

    Public Enum CellCASC
        NRata = 0
        Data = 1
        Importo = 2
        Evasa = 3
        NFC = 4
        DataFC = 5
        ImportoF = 6
        ImportoR = 7
        Serie = 8
    End Enum
    Public Enum CellCAAtt
        Riga = 0
        SerieLotto = 1
        CodArt = 2
        DataSc = 3
        DataEv = 4
        Evasa = 5
        DataScCons = 6
        Fatturata = 7
        SWModAgenti = 8
    End Enum

    Private myTipoDoc As String

    Private DVScadAtt As DataView
    Private DVScadAttNRate As DataView
    Private DVScadAttSAVE As DataView
    Private ArrScadPagCA As ArrayList
    Private ArrScadPagCASAVE As ArrayList 'GIU141223

    Private SqlAdapDocDett As SqlDataAdapter
    Private SqlConnDocDett As SqlConnection
    Private SqlDbSelectCmd As SqlCommand

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
        '''WFP_Vettori1.WucElement = Me

        '''Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        '''SqlDSVettori.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        'giu230218
        myTipoDoc = Session(CSTTIPODOC)
        If IsNothing(myTipoDoc) Then
            myTipoDoc = ""
        End If
        If String.IsNullOrEmpty(myTipoDoc) Then
            myTipoDoc = ""
        End If
        If myTipoDoc = "" Then
            Session(ERROREALL) = SWSI
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore: TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu100118 lo fà gia WUC_Contratti
        ' ''If _WucElement.CKCSTTipoDoc(myTipoDoc) = False Then
        ' ''    _WucElement.Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
        ' ''    Exit Sub
        ' ''End If
        'giu100118 lascio abilitato, idem RitAcconto 
        ' ''GIU080814 SOLO PER:
        ' ''If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Or _
        ' ''    Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Or _
        ' ''    Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
        ' ''    'ok SWFatturaPA SplitIVA
        ' ''Else
        ' ''    Session(CSTSPLITIVA) = False
        ' ''    ChkSplitIVA.Checked = False : ChkSplitIVA.Visible = False
        ' ''End If
        '---------------------------------------------------------------
        If Not IsPostBack Then
            GridViewDettCAAtt.Enabled = False
            GridViewDettCASC.Enabled = False
            SetDDLDettDurNumRiga()
        End If
        DDLPeriodo.Enabled = chkTutteLeDate.Checked
        'giu100118 lo fà WUC_Contratti 
        ' ''If Not IsPostBack Then
        ' ''    SetCdmDAdp()
        ' ''    If Session(SWOP) = SWOPNUOVO Then
        ' ''        AzzeraTxtDocTTD2()
        ' ''    Else
        ' ''        Dim myID As String = Session(IDDOCUMENTI)
        ' ''        If IsNothing(myID) Then
        ' ''            myID = ""
        ' ''        End If
        ' ''        If String.IsNullOrEmpty(myID) Then
        ' ''            myID = ""
        ' ''        End If
        ' ''        If myID = "" Or Not IsNumeric(myID) Then
        ' ''            WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
        ' ''            Exit Sub
        ' ''        End If
        ' ''        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        ' ''        SqlAdapDoc.Fill(DsDocT.DocumentiT)
        ' ''        dvDocT = New DataView(DsDocT.DocumentiT)
        ' ''        PopolaTxtDocTTD2(dvDocT, "")
        ' ''    End If
        ' ''End If
        ''''If Session(F_ANAGRVETTORI_APERTA) = True Then
        ''''    WFP_Vettori1.Show()
        ''''End If
    End Sub

    Public Function PopolaTxtDocTTD2(ByVal _dvDocT As DataView, ByRef strErrore As String) As Boolean
        Try

            ''''GIU100118
            '''Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            '''SqlDSVettori.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
            '---------
            SfondoCampiDocTTD2()
            If _dvDocT.Count = 0 Then
                AzzeraTxtDocTTD2()
                Exit Function
            End If
            'Valuta per i decimali per il calcolo
            Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
            If IsNothing(DecimaliVal) Then DecimaliVal = "2"
            If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
            If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                DecimaliVal = "2" 'Euro
            End If
            '------------------------------------
            'giu120318 SARA' AGGIORNATO DAI DETTAGLI QUI IL CAMPO Totale è il TOTALE DOCUMENTO (Imponibile+Imposta+Spese)
            lblTotMerce.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Totale")), 0, _dvDocT.Item(0).Item("Totale"))
            lblTotMerce.Text = FormattaNumero(CDec(lblTotMerce.Text), Int(DecimaliVal))
            'giu120318 SARA' AGGIORNATO dopo
            ' ''lblTotLordoMercePL.Text = ""
            'GIU050319
            lblTotLordoMerce.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("TotaleM")), 0, _dvDocT.Item(0).Item("TotaleM"))
            lblTotLordoMerce.Text = FormattaNumero(CDec(lblTotLordoMerce.Text), Int(DecimaliVal))
            lblTotDeduzioni.Text = FormattaNumero(CDec(0), Int(DecimaliVal))
            '-
            TxtScontoCassa.AutoPostBack = False
            TxtScontoCassa.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Sconto_Cassa")), 0, _dvDocT.Item(0).Item("Sconto_Cassa"))
            TxtScontoCassa.Text = FormattaNumero(CDec(TxtScontoCassa.Text), Int(DecimaliVal))
            Session(CSTSCCASSA) = TxtScontoCassa.Text.Trim
            TxtScontoCassa.AutoPostBack = True
            '-
            TxtSpeseIncasso.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Spese_Incasso")), 0, _dvDocT.Item(0).Item("Spese_Incasso"))
            TxtSpeseIncasso.Text = FormattaNumero(CDec(TxtSpeseIncasso.Text), Int(DecimaliVal))
            Session(CSTSPINCASSO) = TxtSpeseIncasso.Text.Trim
            '-
            TxtSpeseTrasporto.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Spese_Trasporto")), 0, _dvDocT.Item(0).Item("Spese_Trasporto"))
            TxtSpeseTrasporto.Text = FormattaNumero(CDec(TxtSpeseTrasporto.Text), Int(DecimaliVal))
            Session(CSTSPTRASP) = TxtSpeseTrasporto.Text.Trim
            'GIU260219
            txtBollo.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Bollo")), 0, _dvDocT.Item(0).Item("Bollo"))
            txtBollo.Text = FormattaNumero(CDec(txtBollo.Text), Int(DecimaliVal))
            Session(CSTBOLLO) = txtBollo.Text.Trim
            If Not IsDBNull(_dvDocT.Item(0).Item("BolloACaricoDel")) Then
                DDLSpeseBollo.AutoPostBack = False
                PosizionaItemDDL(_dvDocT.Item(0).Item("BolloACaricoDel").ToString.Trim, DDLSpeseBollo)
                DDLSpeseBollo.AutoPostBack = True
            Else
                DDLSpeseBollo.AutoPostBack = False
                DDLSpeseBollo.SelectedIndex = 0
                DDLSpeseBollo.AutoPostBack = True
            End If
            Session(CSTBOLLOACARICODEL) = DDLSpeseBollo.SelectedValue
            If CDec(txtBollo.Text) > 0 And DDLSpeseBollo.SelectedValue.Trim = "" Then
                txtBollo.BackColor = SEGNALA_KO
            End If
            '-
            txtAbbuono.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Abbuono")), 0, _dvDocT.Item(0).Item("Abbuono"))
            txtAbbuono.Text = FormattaNumero(CDec(txtAbbuono.Text), Int(DecimaliVal))
            Session(CSTABBUONO) = txtAbbuono.Text.Trim
            '-
            TxtSpeseImballo.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Spese_Imballo")), 0, _dvDocT.Item(0).Item("Spese_Imballo"))
            TxtSpeseImballo.Text = FormattaNumero(CDec(TxtSpeseImballo.Text), Int(DecimaliVal))
            Session(CSTSPIMBALLO) = TxtSpeseImballo.Text.Trim
            '-
            TxtDescrizioneImballo.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Descrizione_Imballo")), "", _dvDocT.Item(0).Item("Descrizione_Imballo"))
            '-
            LblIVA1.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Iva1")), "", _dvDocT.Item(0).Item("Iva1"))
            LblImponibile1.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Imponibile1")), 0, _dvDocT.Item(0).Item("Imponibile1"))
            LblImponibile1.Text = FormattaNumero(CDec(LblImponibile1.Text), Int(DecimaliVal))
            LblImposta1.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Imposta1")), 0, _dvDocT.Item(0).Item("Imposta1"))
            LblImposta1.Text = FormattaNumero(CDec(LblImposta1.Text), Int(DecimaliVal))
            '-
            LblIVA2.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Iva2")), "", _dvDocT.Item(0).Item("Iva2"))
            LblImponibile2.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Imponibile2")), 0, _dvDocT.Item(0).Item("Imponibile2"))
            LblImponibile2.Text = FormattaNumero(CDec(LblImponibile2.Text), Int(DecimaliVal))
            LblImposta2.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Imposta2")), 0, _dvDocT.Item(0).Item("Imposta2"))
            LblImposta2.Text = FormattaNumero(CDec(LblImposta2.Text), Int(DecimaliVal))
            '-
            LblIVA3.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Iva3")), "", _dvDocT.Item(0).Item("Iva3"))
            LblImponibile3.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Imponibile3")), 0, _dvDocT.Item(0).Item("Imponibile3"))
            LblImponibile3.Text = FormattaNumero(CDec(LblImponibile3.Text), Int(DecimaliVal))
            LblImposta3.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Imposta3")), 0, _dvDocT.Item(0).Item("Imposta3"))
            LblImposta3.Text = FormattaNumero(CDec(LblImposta3.Text), Int(DecimaliVal))
            '-
            LblIVA4.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Iva4")), "", _dvDocT.Item(0).Item("Iva4"))
            LblImponibile4.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Imponibile4")), 0, _dvDocT.Item(0).Item("Imponibile4"))
            LblImponibile4.Text = FormattaNumero(CDec(LblImponibile4.Text), Int(DecimaliVal))
            LblImposta4.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Imposta4")), 0, _dvDocT.Item(0).Item("Imposta4"))
            LblImposta4.Text = FormattaNumero(CDec(LblImposta4.Text), Int(DecimaliVal))
            '-
            LblTotaleImpon.Text = FormattaNumero(CDec(LblImponibile1.Text) + CDec(LblImponibile2.Text) + CDec(LblImponibile3.Text) + CDec(LblImponibile4.Text), Int(DecimaliVal))
            LblTotaleImposta.Text = FormattaNumero(CDec(LblImposta1.Text) + CDec(LblImposta2.Text) + CDec(LblImposta3.Text) + CDec(LblImposta4.Text), Int(DecimaliVal))
            LblTotDocumento.Text = FormattaNumero(CDec(LblImposta1.Text) + CDec(LblImposta2.Text) + CDec(LblImposta3.Text) + CDec(LblImposta4.Text) + CDec(LblImponibile1.Text) + CDec(LblImponibile2.Text) + CDec(LblImponibile3.Text) + CDec(LblImponibile4.Text), Int(DecimaliVal))

            'Metto spazi nel BLOCCO IVA se uguale a 0
            If CDec(LblImponibile1.Text) = 0 Then LblImponibile1.Text = ""
            If CDec(LblImposta1.Text) = 0 Then LblImposta1.Text = ""
            If CDec(LblImponibile2.Text) = 0 Then LblImponibile2.Text = ""
            If CDec(LblImposta2.Text) = 0 Then LblImposta2.Text = ""
            If CDec(LblImponibile3.Text) = 0 Then LblImponibile3.Text = ""
            If CDec(LblImposta3.Text) = 0 Then LblImposta3.Text = ""
            If CDec(LblImponibile4.Text) = 0 Then LblImponibile4.Text = ""
            If CDec(LblImposta4.Text) = 0 Then LblImposta4.Text = ""

            'DATE DI SCADENZE RATE
            'giu141223 riabbino le scadenze già fatturate
            
            '--------------------------------------------
            Dim NRate As Integer = 0
            Dim TotRate As Decimal = 0 'giu010220
            'giu211223
            Dim TotFatturato As Decimal = 0
            Dim TotResiduo As Decimal = 0
            Try
                If IsDBNull(_dvDocT.Item(0).Item("ScadPagCA")) Then
                    _dvDocT.Item(0).Item("ScadPagCA") = ""
                End If
                ArrScadPagCA = New ArrayList
                Dim myScadCA As ScadPagCAEntity
                If _dvDocT.Item(0).Item("ScadPagCA") <> "" Then
                    Dim lineaSplit As String() = _dvDocT.Item(0).Item("ScadPagCA").Split(";")
                    For i = 0 To lineaSplit.Count - 1
                        If lineaSplit(i).Trim <> "" And (i + 8) <= lineaSplit.Count - 1 Then ' And (i + 6)

                            myScadCA = New ScadPagCAEntity
                            myScadCA.NRata = lineaSplit(i).Trim
                            i += 1
                            myScadCA.Data = lineaSplit(i).Trim
                            i += 1
                            myScadCA.Importo = lineaSplit(i).Trim
                            TotRate += CDec(myScadCA.Importo)
                            i += 1
                            myScadCA.Evasa = lineaSplit(i).Trim
                            i += 1
                            myScadCA.NFC = lineaSplit(i).Trim
                            i += 1
                            myScadCA.DataFC = lineaSplit(i).Trim
                            i += 1
                            myScadCA.Serie = lineaSplit(i).Trim
                            'giu191223
                            i += 1
                            myScadCA.ImportoF = lineaSplit(i).Trim
                            TotFatturato += CDec(myScadCA.ImportoF)
                            i += 1
                            myScadCA.ImportoR = lineaSplit(i).Trim
                            TotResiduo += CDec(myScadCA.ImportoR)
                            ArrScadPagCA.Add(myScadCA)
                            NRate += 1
                        End If
                    Next
                End If
                
                lblNumRate.Text = "Totale rate: " & FormattaNumero(NRate)
                LblTotaleRate.Text = FormattaNumero(TotRate, Int(DecimaliVal))
                lblTotFatturato.Text = FormattaNumero(TotFatturato, Int(DecimaliVal))
                lblTotResiduo.Text = FormattaNumero(TotResiduo, Int(DecimaliVal))

                Session(CSTSCADPAGCA) = ArrScadPagCA
                GridViewDettCASC.DataSource = ArrScadPagCA
                GridViewDettCASC.DataBind()
                
            Catch ex As Exception
                NRate = 0
                TotRate = 0 : TotFatturato = 0 : TotResiduo = 0
                lblNumRate.Text = "ERR.: " + ex.Message.Trim
                lblTotFatturato.Text = "ERRORE"
                lblTotResiduo.Text = "ERRORE"
            End Try
            '-
            ' ''If IsDBNull(_dvDocT.Item(0).Item("ScadAttCA")) Then
            ' ''    _dvDocT.Item(0).Item("ScadAttCA") = ""
            ' ''End If
            ' ''ArrScadAttCA = New ArrayList
            ' ''ArrScadAttCA = Nothing
            ' ''Session(CSTSCADATTCA) = ArrScadAttCA
            ' ''GridViewDettCAAtt.DataSource = ArrScadAttCA
            ' ''GridViewDettCAAtt.DataBind()
            Dim pCodVisita As String = "" : Dim pNVisite As Integer = 0
            If GetCodVisitaDATipoCAIdPag(pCodVisita, pNVisite) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando. Definizione Tipo Contratto errato.", WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
            Try
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
                    ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - IDDOCUMENTO NON VALIDO", WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
                SetCdmDAdp()
                Dim DsContrattiDettALLAtt As New DSDocumenti
                DsContrattiDettALLAtt.ContrattiD.Clear()
                SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
                SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = 1 'fisso per le attività per periodo
                SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
                SqlAdapDocDett.Fill(DsContrattiDettALLAtt.ContrattiD)
                DVScadAtt = New DataView(DsContrattiDettALLAtt.ContrattiD)
                Dim RowD As DSDocumenti.ContrattiDRow
                If DVScadAtt.Count > 0 Then
                    Dim RowsEvasa() As DataRow = DsContrattiDettALLAtt.ContrattiD.Select("")
                    For Each RowD In RowsEvasa
                        RowD.BeginEdit()
                        RowD.Qta_Selezionata = RowD.Qta_Evasa
                        If pCodVisita.Trim.ToUpper = RowD.Cod_Articolo.Trim.ToUpper Then
                            RowD.SWModAgenti = False
                        Else
                            RowD.SWModAgenti = True
                        End If
                        RowD.EndEdit()
                    Next
                    DsContrattiDettALLAtt.AcceptChanges()
                Else
                    _WucElement.btnbtnGeneraAttDNumColorRED(True)
                End If
                '-
                lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
                lblTotaleAtt.Text += " di cui Evase: " & DsContrattiDettALLAtt.ContrattiD.Select("Qta_Evasa<>0").Length.ToString.Trim
                '-
                lblTotaleAtt.Text += " Fatturate: " & DsContrattiDettALLAtt.ContrattiD.Select("Qta_Fatturata<>0").Length.ToString.Trim
                '-

                If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,Serie"
                Session(CSTSCADATTCA) = DVScadAtt
                GridViewDettCAAtt.DataSource = DVScadAtt
                GridViewDettCAAtt.DataBind()
                ' ''SetDDLDettDurNumRiga()
            Catch ex As Exception
                'per adesso proseguo
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore: ", "Caricamento dati attività: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End Try
            '-------------------------------------------------------------------------------------------------
            'GIU050118 da qui in poi determino il totale netto a pagare, tenendo conto dello SPLIT/RIT.ACCONTO
            '-------------------------------------------------------------------------------------------------
            If IsDBNull(_dvDocT.Item(0).Item("SplitIVA")) Then
                ChkSplitIVA.AutoPostBack = False
                ChkSplitIVA.Checked = False
                ChkSplitIVA.AutoPostBack = True
            Else
                ChkSplitIVA.AutoPostBack = False
                ChkSplitIVA.Checked = _dvDocT.Item(0).Item("SplitIVA")
                ChkSplitIVA.AutoPostBack = True
            End If
            '.giu200420
            If IsDBNull(_dvDocT.Item(0).Item("SWTipoEvTotale")) Then
                chkNoDivTotRate.AutoPostBack = False
                chkNoDivTotRate.Checked = False
                chkNoDivTotRate.AutoPostBack = True
            Else
                chkNoDivTotRate.AutoPostBack = False
                chkNoDivTotRate.Checked = _dvDocT.Item(0).Item("SWTipoEvTotale")
                chkNoDivTotRate.AutoPostBack = True
            End If
            '-
            If IsDBNull(_dvDocT.Item(0).Item("SWTipoEvSaldo")) Then
                chkAccorpaRateAA.AutoPostBack = False
                chkAccorpaRateAA.Checked = False
                chkAccorpaRateAA.AutoPostBack = True
            Else
                chkAccorpaRateAA.AutoPostBack = False
                chkAccorpaRateAA.Checked = _dvDocT.Item(0).Item("SWTipoEvSaldo")
                chkAccorpaRateAA.AutoPostBack = True
            End If
            '----------------
            'GIU180219
            If _dvDocT.Item(0).Item("IDAnagrProvv").ToString.Trim = "" Then
                Dim myCodCli As String = _dvDocT.Item(0).Item("Cod_Cliente").ToString
                If myCodCli.Trim <> "" Then
                    Dim mySplitIVA As Boolean = False
                    Call Documenti.CKClientiIPAByIDConORCod(0, myCodCli.Trim, mySplitIVA, strErrore)
                    If strErrore.Trim <> "" Then
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ' ''ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
                        ' ''Exit Sub
                        'non mi frega
                    ElseIf ChkSplitIVA.Checked <> mySplitIVA Then
                        ChkSplitIVA.BackColor = SEGNALA_KO
                    Else
                        ChkSplitIVA.BackColor = SEGNALA_OK
                    End If
                End If
            End If
            '-
            '''If IsDBNull(_dvDocT.Item(0).Item("RitAcconto")) Then
            '''    chkRitAcconto.AutoPostBack = False
            '''    chkRitAcconto.Checked = False
            '''    chkRitAcconto.AutoPostBack = True
            '''Else
            '''    chkRitAcconto.AutoPostBack = False
            '''    chkRitAcconto.Checked = _dvDocT.Item(0).Item("RitAcconto")
            '''    chkRitAcconto.AutoPostBack = True
            '''End If
            ''''-
            '''If chkRitAcconto.Checked = False Then
            '''    txtImponibileRA.Text = ""
            '''    txtImponibileRA.Enabled = False
            '''    txtPercRA.Text = ""
            '''    txtPercRA.Enabled = False
            '''    LblTotaleRA.Text = ""
            '''Else
            '''    If IsDBNull(_dvDocT.Item(0).Item("ImponibileRA")) Then
            '''        txtImponibileRA.Text = FormattaNumero(0, Int(DecimaliVal))
            '''    Else
            '''        txtImponibileRA.Text = FormattaNumero(_dvDocT.Item(0).Item("ImponibileRA"), Int(DecimaliVal))
            '''    End If
            '''    If IsDBNull(_dvDocT.Item(0).Item("PercRA")) Then
            '''        txtPercRA.Text = FormattaNumero(0, Int(DecimaliVal))
            '''    Else
            '''        txtPercRA.Text = FormattaNumero(_dvDocT.Item(0).Item("PercRA"), Int(DecimaliVal))
            '''    End If
            '''    If IsDBNull(_dvDocT.Item(0).Item("TotaleRA")) Then
            '''        LblTotaleRA.Text = FormattaNumero(0, Int(DecimaliVal))
            '''    Else
            '''        LblTotaleRA.Text = FormattaNumero(_dvDocT.Item(0).Item("TotaleRA"), Int(DecimaliVal))
            '''    End If
            '''End If
            If IsDBNull(_dvDocT.Item(0).Item("TotNettoPagare")) Then
                LblTotNettoPagare.Text = FormattaNumero(0, Int(DecimaliVal))
            Else
                LblTotNettoPagare.Text = FormattaNumero(_dvDocT.Item(0).Item("TotNettoPagare"), Int(DecimaliVal))
            End If
            '---------
        Catch ex As Exception
            Session(ERROREALL) = SWSI
            Session(ERRORE) = "(PopolaTxtDocTTD2) " & ex.Message.Trim
        End Try
    End Function
    Public Function SaveScadenze() As Boolean
        'giu151223
        SaveScadenze = False
        If Not (Session(CSTSCADPAGCA) Is Nothing) Then
            ArrScadPagCA = New ArrayList
            ArrScadPagCA = Session(CSTSCADPAGCA)
            Dim myScad As New ScadPagCAEntity
            '-
            ArrScadPagCASAVE = New ArrayList
            Dim myScadSAVE As New ScadPagCAEntity
            For I = 0 To ArrScadPagCA.Count - 1
                myScadSAVE = New ScadPagCAEntity
                myScadSAVE = ArrScadPagCA(I)
                ArrScadPagCASAVE.Add(myScadSAVE)
            Next
            Session(CSTSCADPAGCA + "SAVE") = ArrScadPagCASAVE
            SaveScadenze = True
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '-------
    End Function
    Public Function RestoreScadenze() As Boolean
        'GIU141223 ABBINAMENTO SCADENZE FATTURATE IN PRECEDENZA
        Dim SWOKModScadPAG As Boolean = False
        Dim x As Integer = 0 : Dim xx As Integer = 0
        Try
            If Not (Session(CSTSCADPAGCA + "SAVE") Is Nothing) Then
                ArrScadPagCASAVE = New ArrayList
                ArrScadPagCASAVE = Session(CSTSCADPAGCA + "SAVE")
                Dim myScad As New ScadPagCAEntity
                Dim myScadSAVE As New ScadPagCAEntity
                For x = 0 To ArrScadPagCASAVE.Count - 1
                    myScadSAVE = ArrScadPagCASAVE(x)
                    For xx = 0 To ArrScadPagCA.Count - 1
                        myScad = ArrScadPagCA(xx)
                        If CDate(myScadSAVE.Data).Year = CDate(myScad.Data).Year And myScadSAVE.Importo = myScad.Importo And _
                            myScadSAVE.Serie = myScad.Serie And _
                           (myScadSAVE.Evasa <> myScad.Evasa Or myScadSAVE.NFC <> myScad.NFC Or myScadSAVE.DataFC <> myScad.DataFC Or _
                            myScadSAVE.ImportoF <> myScad.ImportoF) Then
                            SWOKModScadPAG = True
                            myScad.Evasa = myScadSAVE.Evasa
                            myScad.NFC = myScadSAVE.NFC
                            myScad.DataFC = myScadSAVE.DataFC
                            myScad.ImportoF = myScadSAVE.ImportoF
                            myScad.ImportoR = myScadSAVE.ImportoR
                        End If
                    Next
                Next
            Else
                'nulla aspetto l'aggiornamento
            End If
            '-------
        Catch ex As Exception
            'nulla
        End Try
        Session(CSTSCADPAGCA + "SAVE") = ""
        Session("GeneraAttPeriodi") = SWNO
        '------------------------------------------------------
        If SWOKModScadPAG Then
            Session(CSTSCADPAGCA) = ArrScadPagCA
            GridViewDettCASC.DataSource = ArrScadPagCA
            GridViewDettCASC.DataBind()
            '-------
            
            TabContainer3.ActiveTabIndex = 1
            '----
            Session(SWOP) = SWOPMODSCATT
            Session(SWOPMODSCATT) = SWSI
            GridViewDettCASC.Enabled = True
            btnModScPagCA.Visible = False
            btnAggScPagCA.Visible = True
            btnAnnScPagCA.Visible = True
            btnAggFCFromOC.Enabled = False
            btnAggDataScAtt.Enabled = False
            lblMessRatePag.Text = "ATTENZIONE ricalcolo e Riporto fatture su Scadenze, alcune rate sono state fatturate in precedenza.<br>SI PREGA DI VERIFICARE."
        End If
    End Function
    Public Sub AzzeraTxtDocTTD2()
        SfondoCampiDocTTD2()
        '------------------------------------
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        '------------------------------------
        Dim DecimaliValutaFinito As Integer = Int(DecimaliVal)

        lblTotMerce.Text = FormattaNumero(0, DecimaliValutaFinito)
        lblTotLordoMerce.Text = FormattaNumero(0, DecimaliValutaFinito)
        lblTotDeduzioni.Text = FormattaNumero(0, DecimaliValutaFinito)
        ' ''lblTotLordoMercePL.Text = ""
        '-
        TxtScontoCassa.AutoPostBack = False
        TxtScontoCassa.Text = FormattaNumero(0, DecimaliValutaFinito)
        Session(CSTSCCASSA) = TxtScontoCassa.Text.Trim
        TxtScontoCassa.AutoPostBack = True
        '-
        TxtSpeseIncasso.Text = FormattaNumero(0, DecimaliValutaFinito)
        Session(CSTSPINCASSO) = TxtSpeseIncasso.Text.Trim
        '-
        TxtSpeseTrasporto.Text = FormattaNumero(0, DecimaliValutaFinito)
        Session(CSTSPTRASP) = TxtSpeseTrasporto.Text.Trim
        '-
        txtAbbuono.Text = FormattaNumero(0, DecimaliValutaFinito)
        Session(CSTABBUONO) = txtAbbuono.Text.Trim
        '-
        TxtSpeseImballo.Text = FormattaNumero(0, DecimaliValutaFinito)
        Session(CSTSPIMBALLO) = TxtSpeseImballo.Text.Trim
        'GIU260219
        txtBollo.Text = FormattaNumero(0, DecimaliValutaFinito)
        Session(CSTBOLLO) = txtBollo.Text.Trim
        DDLSpeseBollo.AutoPostBack = False
        DDLSpeseBollo.SelectedIndex = 0
        DDLSpeseBollo.AutoPostBack = True
        Session(CSTBOLLOACARICODEL) = DDLSpeseBollo.SelectedValue
        '-
        TxtDescrizioneImballo.Text = ""
        '-
        LblIVA1.Text = ""
        LblImponibile1.Text = ""
        LblImposta1.Text = ""
        '-
        LblIVA2.Text = ""
        LblImponibile2.Text = ""
        LblImposta2.Text = ""
        '-
        LblIVA3.Text = ""
        LblImponibile3.Text = ""
        LblImposta3.Text = ""
        '-
        LblIVA4.Text = ""
        LblImponibile4.Text = ""
        LblImposta4.Text = ""
        '-
        LblTotaleImpon.Text = ""
        LblTotaleImposta.Text = ""
        LblTotDocumento.Text = ""
        'giu010220
        Session(CSTSCADPAGCA) = Nothing
        GridViewDettCASC.DataSource = Nothing
        GridViewDettCASC.DataBind()
        '-
        Session(CSTSCADATTCA) = Nothing
        GridViewDettCAAtt.DataSource = Nothing
        GridViewDettCAAtt.DataBind()
        lblTotaleAtt.Text = ""
        '-
        'DATE DI SCADENZE RATE
        ' ''lblDataScad1.Text = ""
        ' ''lblDataScad2.Text = ""
        ' ''lblDataScad3.Text = ""
        ' ''lblDataScad4.Text = ""
        ' ''lblDataScad5.Text = ""
        '' ''IMPORTO RATE DI SCADENZE RATE
        ' ''lblImpRata1.Text = ""
        ' ''lblImpRata2.Text = ""
        ' ''lblImpRata3.Text = ""
        ' ''lblImpRata4.Text = ""
        ' ''lblImpRata5.Text = ""
        LblTotaleRate.Text = ""
        'giu211217 GIU120118
        'GIU0301219 SpliIVA arriva se richiesto da SESSIONE
        Try
            ChkSplitIVA.AutoPostBack = False
            ChkSplitIVA.Checked = Session(CSTSPLITIVA)
            ChkSplitIVA.AutoPostBack = True
            '-
            chkNoDivTotRate.AutoPostBack = False
            chkNoDivTotRate.Checked = False
            chkNoDivTotRate.AutoPostBack = True
            '-
            chkAccorpaRateAA.AutoPostBack = False
            chkAccorpaRateAA.Checked = False
            chkAccorpaRateAA.AutoPostBack = True
        Catch ex As Exception
            ChkSplitIVA.AutoPostBack = False
            ChkSplitIVA.Checked = False
            Session(CSTSPLITIVA) = False
            ChkSplitIVA.AutoPostBack = True
        End Try
        '--------------------------------------------------
        '''txtImponibileRA.Text = ""
        '''txtPercRA.Text = ""
        '''LblTotaleRA.Text = ""
        LblTotNettoPagare.Text = ""
        '''chkRitAcconto.AutoPostBack = False
        '''chkRitAcconto.Checked = False
        '''chkRitAcconto.AutoPostBack = True
        '---------
    End Sub
    Public Sub SfondoCampiDocTTD2()
        TxtScontoCassa.BackColor = SEGNALA_OK
        TxtSpeseIncasso.BackColor = SEGNALA_OK
        TxtSpeseTrasporto.BackColor = SEGNALA_OK
        'GIU260219
        txtBollo.BackColor = SEGNALA_OK
        DDLSpeseBollo.BackColor = SEGNALA_OK
        '-
        txtAbbuono.BackColor = SEGNALA_OK
        TxtSpeseImballo.BackColor = SEGNALA_OK
        TxtDescrizioneImballo.BackColor = SEGNALA_OK
        'giu211217 GIU050118 GIU260419
        LblTotaleImpon.BackColor = SEGNALA_OKLBL : LblTotaleImposta.BackColor = SEGNALA_OKLBL
        LblTotDocumento.BackColor = SEGNALA_OKLBL
        ChkSplitIVA.BackColor = SEGNALA_OK
        chkNoDivTotRate.BackColor = SEGNALA_OK : chkAccorpaRateAA.BackColor = SEGNALA_OK
        'chkRitAcconto.BackColor = SEGNALA_OK
        'txtImponibileRA.BackColor = SEGNALA_OK
        'txtPercRA.BackColor = SEGNALA_OK
        'LblTotaleRA.BackColor = SEGNALA_OKLBL
        LblTotNettoPagare.BackColor = SEGNALA_OKLBL
        LblTotaleRate.BackColor = SEGNALA_OKLBL

    End Sub

    Public Sub SetEnableTxt(ByVal Valore As Boolean)
        TxtScontoCassa.Enabled = Valore
        TxtSpeseIncasso.Enabled = Valore
        TxtSpeseTrasporto.Enabled = Valore
        txtBollo.Enabled = Valore
        DDLSpeseBollo.Enabled = Valore
        txtAbbuono.Enabled = Valore
        TxtSpeseImballo.Enabled = Valore
        TxtDescrizioneImballo.Enabled = Valore
        'giu211217
        ChkSplitIVA.Enabled = Valore
        chkNoDivTotRate.Enabled = Valore : chkAccorpaRateAA.Enabled = Valore
        'txtImponibileRA.Enabled = Valore
        'txtPercRA.Enabled = Valore
        'chkRitAcconto.Enabled = Valore
        If Valore = False Then
            GridViewDettCASC.Enabled = Valore
            GridViewDettCAAtt.Enabled = Valore
        End If
        btnAggFCFromOC.Enabled = Not Valore
        btnAggDataScAtt.Enabled = Not Valore
        btnModScPagCA.Enabled = Not Valore
        btnAggScPagCA.Enabled = Not Valore
        btnAnnScPagCA.Enabled = Not Valore
        '-
        btnModScAttCA.Enabled = Not Valore
        btnModScAttCA.Enabled = Not Valore
        btnModScAttCA.Enabled = Not Valore

    End Sub

    Public Function GetDati(ByRef TotMerce As Decimal, _
                            ByRef ScCassa As Decimal, _
                            ByRef TotMerceLordo As Decimal, _
                            ByRef SpIncasso As Decimal, _
                            ByRef SpTrasp As Decimal, _
                            ByRef Abbuono As Decimal, _
                            ByRef SpImballo As Decimal, _
                            ByRef DesImballo As String, _
                            ByRef Iva() As Integer, _
                            ByRef Imponibile() As Decimal, _
                            ByRef Imposta() As Decimal, _
                            ByRef DataScad() As String, _
                            ByRef ImpRata() As Decimal, _
                            ByRef TipoSped As String, _
                            ByRef Vettori() As Long, _
                            ByRef Porto As String, _
                            ByRef DesPorto As String, _
                            ByRef Aspetto As String, _
                            ByRef Colli As Integer, _
                            ByRef Pezzi As Integer, _
                            ByRef Peso As Decimal, _
                            ByRef SWNoDivTotRate As Boolean, _
                            ByRef SWAccorpaRateAA As Boolean, _
                            ByRef SplitIVA As Boolean, _
                            ByRef RitAcconto As Boolean, _
                            ByRef ImponibileRA As Decimal, _
                            ByRef PercRA As Decimal, _
                            ByRef TotaleRA As Decimal, _
                            ByRef TotNettoPagare As Decimal, _
                            ByRef Bollo As Decimal, _
                            ByRef BolloACaricoDel As String) As Boolean
        GetDati = True
        Try
            TotMerce = CDec(lblTotMerce.Text.Trim)
        Catch ex As Exception
            TotMerce = 0
        End Try
        Try
            If TxtScontoCassa.Text.Trim = "" Then TxtScontoCassa.Text = "0"
            ScCassa = CDec(TxtScontoCassa.Text.Trim)
        Catch ex As Exception
            ScCassa = 0
            GetDati = False : TxtScontoCassa.BackColor = SEGNALA_KO
        End Try
        Session(CSTSCCASSA) = ScCassa
        Try
            TotMerceLordo = CDec(lblTotLordoMerce.Text.Trim)
        Catch ex As Exception
            TotMerceLordo = 0
        End Try
        Try
            If TxtSpeseIncasso.Text.Trim = "" Then TxtSpeseIncasso.Text = "0,00"
            SpIncasso = CDec(TxtSpeseIncasso.Text.Trim)
        Catch ex As Exception
            SpIncasso = 0
            GetDati = False : TxtSpeseIncasso.BackColor = SEGNALA_KO
        End Try
        Session(CSTSPINCASSO) = SpIncasso
        Try
            If TxtSpeseTrasporto.Text.Trim = "" Then TxtSpeseTrasporto.Text = "0,00"
            SpTrasp = CDec(TxtSpeseTrasporto.Text.Trim)
        Catch ex As Exception
            SpTrasp = 0
            GetDati = False : TxtSpeseTrasporto.BackColor = SEGNALA_KO
        End Try
        Session(CSTSPTRASP) = SpTrasp
        'GIU260219
        Try
            If txtBollo.Text.Trim = "" Then txtBollo.Text = "0,00"
            Bollo = CDec(txtBollo.Text.Trim)
        Catch ex As Exception
            Bollo = 0
            GetDati = False : txtBollo.BackColor = SEGNALA_KO
        End Try
        Session(CSTBOLLO) = Bollo
        Try
            BolloACaricoDel = DDLSpeseBollo.SelectedValue
        Catch ex As Exception
            BolloACaricoDel = ""
            GetDati = False : DDLSpeseBollo.BackColor = SEGNALA_KO
        End Try
        Session(CSTBOLLOACARICODEL) = BolloACaricoDel
        '---------
        Try
            If txtAbbuono.Text.Trim = "" Then txtAbbuono.Text = "0"
            Abbuono = CDec(txtAbbuono.Text.Trim)
        Catch ex As Exception
            Abbuono = 0
            GetDati = False : txtAbbuono.BackColor = SEGNALA_KO
        End Try
        Session(CSTABBUONO) = Abbuono
        Try
            If TxtSpeseImballo.Text.Trim = "" Then TxtSpeseImballo.Text = "0"
            SpImballo = CDec(TxtSpeseImballo.Text.Trim)
        Catch ex As Exception
            SpImballo = 0
            GetDati = False : TxtSpeseImballo.BackColor = SEGNALA_KO
        End Try
        Session(CSTSPIMBALLO) = SpImballo
        DesImballo = TxtDescrizioneImballo.Text.Trim
        Dim Cont As Integer
        For Cont = 0 To 3
            Iva(Cont) = 0 : Imponibile(Cont) = 0 : Imposta(Cont) = 0
        Next
        Try
            LblIVA1.BackColor = SEGNALA_OKLBL
            For Cont = 0 To 3
                Select Case Cont
                    Case 0
                        If LblIVA1.Text.Trim = "" Then LblIVA1.Text = "0"
                        Iva(Cont) = IIf(LblIVA1.Text.Trim <> "", LblIVA1.Text.Trim, 0)
                        If LblImponibile1.Text.Trim = "" Then LblImponibile1.Text = "0"
                        Imponibile(Cont) = IIf(LblImponibile1.Text.Trim <> "", CDec(LblImponibile1.Text.Trim), 0)
                        If LblImposta1.Text.Trim = "" Then LblImposta1.Text = "0"
                        Imposta(Cont) = IIf(LblImposta1.Text.Trim <> "", CDec(LblImposta1.Text.Trim), 0)
                    Case 1
                        If LblIVA2.Text.Trim = "" Then LblIVA2.Text = "0"
                        Iva(Cont) = IIf(LblIVA2.Text.Trim <> "", LblIVA2.Text.Trim, 0)
                        If LblImponibile2.Text.Trim = "" Then LblImponibile2.Text = "0"
                        Imponibile(Cont) = IIf(LblImponibile2.Text.Trim <> "", CDec(LblImponibile2.Text.Trim), 0)
                        If LblImposta2.Text.Trim = "" Then LblImposta2.Text = "0"
                        Imposta(Cont) = IIf(LblImposta2.Text.Trim <> "", CDec(LblImposta2.Text.Trim), 0)
                    Case 2
                        If LblIVA3.Text.Trim = "" Then LblIVA3.Text = "0"
                        Iva(Cont) = IIf(LblIVA3.Text.Trim <> "", LblIVA3.Text.Trim, 0)
                        If LblImponibile3.Text.Trim = "" Then LblImponibile3.Text = "0"
                        Imponibile(Cont) = IIf(LblImponibile3.Text.Trim <> "", CDec(LblImponibile3.Text.Trim), 0)
                        If LblImposta3.Text.Trim = "" Then LblImposta3.Text = "0"
                        Imposta(Cont) = IIf(LblImposta3.Text.Trim <> "", CDec(LblImposta3.Text.Trim), 0)
                    Case 3
                        If LblIVA4.Text.Trim = "" Then LblIVA4.Text = "0"
                        Iva(Cont) = IIf(LblIVA4.Text.Trim <> "", LblIVA4.Text.Trim, 0)
                        If LblImponibile4.Text.Trim = "" Then LblImponibile4.Text = "0"
                        Imponibile(Cont) = IIf(LblImponibile4.Text.Trim <> "", CDec(LblImponibile4.Text.Trim), 0)
                        If LblImposta4.Text.Trim = "" Then LblImposta4.Text = "0"
                        Imposta(Cont) = IIf(LblImposta4.Text.Trim <> "", CDec(LblImposta4.Text.Trim), 0)
                End Select
            Next Cont
        Catch ex As Exception
            GetDati = False : LblIVA1.BackColor = SEGNALA_KO
        End Try
        '-
        SplitIVA = ChkSplitIVA.Checked
        Session(CSTSPLITIVA) = SplitIVA
        SWNoDivTotRate = chkNoDivTotRate.Checked
        SWAccorpaRateAA = chkAccorpaRateAA.Checked
        RitAcconto = False 'chkRitAcconto.Checked
        ImponibileRA = 0 ': txtImponibileRA.Text = ""
        PercRA = 0 ': txtPercRA.Text = ""
        TotaleRA = 0 ': LblTotaleRA.Text = ""
        '-
        Try
            If LblTotNettoPagare.Text.Trim = "" Then LblTotNettoPagare.Text = "0"
            TotNettoPagare = CDec(LblTotNettoPagare.Text.Trim)
            If LblTotDocumento.Text.Trim = "" Then LblTotDocumento.Text = "0"
            If lblTotDeduzioni.Text.Trim = "" Then lblTotDeduzioni.Text = "0"
            If lblTotLordoMerce.Text.Trim = "" Then lblTotLordoMerce.Text = "0" 'GIU290519
            If TotNettoPagare = 0 And (CDec(LblTotDocumento.Text) = 0 Or CDec(LblTotDocumento.Text) < 0) And CDec(lblTotDeduzioni.Text) = 0 And CDec(lblTotLordoMerce.Text) = 0 Then
                ' ''GIU070220 GetDati = False
                LblTotDocumento.BackColor = SEGNALA_KO
                LblTotNettoPagare.BackColor = SEGNALA_KO
            End If
        Catch ex As Exception
            TotNettoPagare = 0
            GetDati = False : LblTotNettoPagare.BackColor = SEGNALA_KO
        End Try

    End Function

    Public Function CalcolaTotSpeseScad(ByVal dsDocDett As DataSet, ByRef Iva() As Integer, ByRef Imponibile() As Decimal, ByRef Imposta() As Decimal, ByVal DecimaliValuta As Integer, _
                                        ByRef MoltiplicatoreValuta As Integer, ByRef Totale As Decimal, ByRef TotaleLordoMerce As Decimal, ByVal ScontoCassa As Decimal, _
                                        Optional ByVal Listino As Long = 9999, Optional ByVal TipoDocumento As String = "CA", Optional ByVal Abbuono As Decimal = 0, _
                                        Optional ByRef strErrore As String = "", Optional ByRef TotaleLordoMercePL As Decimal = 0, _
                                        Optional ByRef Deduzioni As Decimal = 0) As Boolean 'giu020519
        'TD3 SPESE,TRASPORTO, TOTALE AGGIORNARE
        Call SfondoCampiDocTTD2() 'GIU260419
        Dim Cont As Integer
        lblTotMerce.Text = FormattaNumero(Totale, DecimaliValuta)
        lblTotLordoMerce.Text = FormattaNumero(TotaleLordoMerce, DecimaliValuta)
        ' ''lblTotLordoMercePL.Text = FormattaNumero(TotaleLordoMercePL, DecimaliValuta)
        lblTotDeduzioni.Text = FormattaNumero(Deduzioni, DecimaliValuta) 'giu020519

        For Cont = 0 To 3
            Select Case Cont
                Case 0
                    LblIVA1.Text = IIf(Iva(Cont) <> 0, Iva(Cont), "")
                    If Imponibile(Cont) <> 0 Then
                        LblImponibile1.Text = FormattaNumero(Imponibile(Cont), DecimaliValuta)
                    Else
                        LblImponibile1.Text = ""
                    End If
                    If Imposta(Cont) <> 0 Then
                        LblImposta1.Text = FormattaNumero(Imposta(Cont), DecimaliValuta)
                    Else
                        LblImposta1.Text = ""
                    End If
                Case 1
                    LblIVA2.Text = IIf(Iva(Cont) <> 0, Iva(Cont), "")
                    If Imponibile(Cont) <> 0 Then
                        LblImponibile2.Text = FormattaNumero(Imponibile(Cont), DecimaliValuta)
                    Else
                        LblImponibile2.Text = ""
                    End If
                    If Imposta(Cont) <> 0 Then
                        LblImposta2.Text = FormattaNumero(Imposta(Cont), DecimaliValuta)
                    Else
                        LblImposta2.Text = ""
                    End If
                Case 2
                    LblIVA3.Text = IIf(Iva(Cont) <> 0, Iva(Cont), "")
                    If Imponibile(Cont) <> 0 Then
                        LblImponibile3.Text = FormattaNumero(Imponibile(Cont), DecimaliValuta)
                    Else
                        LblImponibile3.Text = ""
                    End If
                    If Imposta(Cont) <> 0 Then
                        LblImposta3.Text = FormattaNumero(Imposta(Cont), DecimaliValuta)
                    Else
                        LblImposta3.Text = ""
                    End If
                Case 3
                    LblIVA4.Text = IIf(Iva(Cont) <> 0, Iva(Cont), "")
                    If Imponibile(Cont) <> 0 Then
                        LblImponibile4.Text = FormattaNumero(Imponibile(Cont), DecimaliValuta)
                    Else
                        LblImponibile4.Text = ""
                    End If
                    If Imposta(Cont) <> 0 Then
                        LblImposta4.Text = FormattaNumero(Imposta(Cont), DecimaliValuta)
                    Else
                        LblImposta4.Text = ""
                    End If
            End Select

        Next Cont

        LblTotaleImpon.Text = FormattaNumero(Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3), DecimaliValuta)
        LblTotaleImposta.Text = FormattaNumero(Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3), DecimaliValuta)
        LblTotDocumento.Text = FormattaNumero(Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3), DecimaliValuta)

        'giu150118 Calcolo LblTotNettoPagare
        If ChkSplitIVA.Checked = True Then
            LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotaleImpon.Text), DecimaliValuta)
        Else
            LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotDocumento.Text), DecimaliValuta)
        End If
        '-
        Dim myTot As Decimal = 0
        Try
            myTot = CDec(txtAbbuono.Text)
        Catch ex As Exception
            myTot = 0
        End Try
        If myTot <> 0 Then
            LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotNettoPagare.Text) + myTot, DecimaliValuta)
        End If
        '----------------------------------------------------
        strErrore = ""
        If Calcola_ScadenzeCA(TipoDocumento, DecimaliValuta, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Function
        End If

    End Function

    Private Function Calcola_ScadenzeCA(ByVal TipoDocumento As String, ByVal DecimaliValuta As Integer, ByRef strErrore As String) As Boolean
        Calcola_ScadenzeCA = True
        strErrore = ""
        'giu020320 blocco RICALCOLO SCADENZE PER EFFETTO DI PAGAMENTI GIA' EFFETTUATI
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            Calcola_ScadenzeCA = False
            strErrore = "Errore: SESSIONE SCADENZE SCADUTA (ID) - Riprovate la modifica uscendo e rientrando"
            Exit Function
        End If
        '-
        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        'giu020320 @@@@@@@@@@@@@@@@@@ CHIAMATA UNICA QUI DENTRO
        Dim myNonCompleto As String = Session(CSTNONCOMPLETO)
        If IsNothing(myNonCompleto) Then
            myNonCompleto = ""
        End If
        If String.IsNullOrEmpty(myNonCompleto) Then
            myNonCompleto = ""
        End If
        'giu281023 non è possibile dividere per periodo se è definito "U"nica Rata per periodo
        Dim TipoFatt As String = Session(CSTTIPOFATT)
        If String.IsNullOrEmpty(TipoFatt) Then TipoFatt = ""
        If TipoFatt = "" Then
            Calcola_ScadenzeCA = False
            strErrore = "Errore: SESSIONE SCADUTA (Tipo Fatturazione non definito) - Riprovate la modifica uscendo e rientrando"
            Exit Function
        End If
        lblMessRatePag.Text = "" ': lblMessRatePag.Visible = False
        If TipoFatt = "U" Then 'GIU281023
            lblMessRatePag.Text = "Ricalcolo Tot.Rate Scadenze per periodo !! Unica 1° Rata !!"
            chkNoDivTotRate.AutoPostBack = False
            chkNoDivTotRate.Checked = False
            chkNoDivTotRate.AutoPostBack = True
            '-
            chkAccorpaRateAA.AutoPostBack = False
            chkAccorpaRateAA.Checked = False
            chkAccorpaRateAA.AutoPostBack = True
        End If
        '-
        'giu300722
        Dim SWOKPagScad As Boolean
        If myNonCompleto <> SWNO Then
            '''lblMessRatePag.Text = "IMPOSSIBILE il ricalcolo Scadenze, il contratto risulta INCOMPLETO."
            '''lblMessRatePag.Visible = True : lblTotaleFatturato.Visible = False
            '''Exit Function
        End If
        If CKAttEvPagEv(, , SWOKPagScad) = True And myNonCompleto = SWNO Then
            If SWOKPagScad = True Then
                '''lblMessRatePag.Text = "IMPOSSIBILE il ricalcolo Scadenze, alcune scadenze sono state fatturate."
                '''Exit Function
                'giu151223
                If Session(CSTNONCOMPLETO) = SWSI Or Session(CSTSTATODOC) = "5" Then
                    '''lblMessRatePag.Text = "IMPOSSIBILE il ricalcolo Scadenze, il contratto risulta INCOMPLETO."
                    '''Exit Function
                ElseIf Session("GeneraAttPeriodi") = SWSI Then
                    'ok ricolcolo
                Else
                    lblMessRatePag.Text = "IMPOSSIBILE il ricalcolo Scadenze, alcune scadenze sono state fatturate."
                    Exit Function
                End If
            End If
        End If
        '-
        Dim NumRate As Integer = 0 : Dim NumGG As Integer = 0
        Dim TotRate As Decimal = 0
        Dim ind As Integer = 0
        Session(CSTSCADPAGCA) = Nothing
        GridViewDettCASC.DataSource = Nothing
        GridViewDettCASC.DataBind()
        LblTotaleRate.Text = ""

        If LblTotDocumento.Text.Trim = "" Then
            Exit Function
        ElseIf CDec(LblTotDocumento.Text) = 0 Then
            Exit Function
        End If
        'Data Documento (da TD0)
        Dim DataDoc As String = Session(CSTDATADOC)
        If IsNothing(DataDoc) Then DataDoc = ""
        If String.IsNullOrEmpty(DataDoc) Then DataDoc = ""
        If Not IsDate(DataDoc) Then DataDoc = ""
        If DataDoc = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Data documento obbligatoria per il calcolo Scadenze", WUC_ModalPopup.TYPE_ERROR)
            Calcola_ScadenzeCA = False
            Exit Function
        End If
        'GIU290120
        Dim DataInizio As String = Session(CSTDATAINIZIO)
        If IsNothing(DataInizio) Then DataInizio = ""
        If String.IsNullOrEmpty(DataInizio) Then DataInizio = ""
        If Not IsDate(DataInizio) Then DataInizio = ""
        Dim DataFine As String = Session(CSTDATAFINE)
        If IsNothing(DataFine) Then DataFine = ""
        If String.IsNullOrEmpty(DataFine) Then DataFine = ""
        If Not IsDate(DataFine) Then DataFine = ""
        Dim DataAccetta As String = Session(CSTDATAACCETTA)
        If IsNothing(DataAccetta) Then DataAccetta = ""
        If String.IsNullOrEmpty(DataAccetta) Then DataAccetta = ""
        If Not IsDate(DataAccetta) Then DataAccetta = ""
        'Codice Pagamento (da TD0)
        Dim CodPag As String = Session(CSTIDPAG)
        If IsNothing(CodPag) Then CodPag = ""
        If String.IsNullOrEmpty(CodPag) Then CodPag = ""
        If Not IsNumeric(CodPag) Then CodPag = ""
        If CodPag = "" Or CodPag = "0" Then Exit Function
        '-
        Dim dsPag As New DataSet
        Dim rowPag() As DataRow
        strSQL = "Select * From TipoContratto WHERE Codice = " & CodPag.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsPag)
            If (dsPag.Tables.Count > 0) Then
                If (dsPag.Tables(0).Rows.Count > 0) Then
                    rowPag = dsPag.Tables(0).Select()
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Codice nella tabella TipoContratto: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Calcola_ScadenzeCA = False
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Codice nella tabella TipoContratto: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                Calcola_ScadenzeCA = False
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura TipoContratto: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Calcola_ScadenzeCA = False
            Exit Function
        End Try
        'giu020124 eseguo sempre per attivita 
        If Calcola_ScadenzeCATP4(rowPag, TipoDocumento, DecimaliValuta, NumRate, strErrore) = False Then
            _WucElement.SetTxtlblMessDoc("(CSCATP4) Errore: Nella compilazione Scadenze" & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, ""))
            Calcola_ScadenzeCA = False
            Exit Function
        End If
        Exit Function
        '------------------------------------------------------------------------------------------------------------------------------------
        '- ok rate ora la data da tabella TipoContratto
        Dim DataCalcoloSc As String = DataDoc
        Dim myMess As String = "Data documento"
        If rowPag(0).Item("TipoScadenza") = 1 Then
            DataCalcoloSc = DataDoc
            myMess = "Data documento"
        ElseIf rowPag(0).Item("TipoScadenza") = 2 Then
            DataCalcoloSc = DataAccetta
            myMess = "Data Accettazione contratto"
        ElseIf rowPag(0).Item("TipoScadenza") = 3 Then
            DataCalcoloSc = DataInizio
            myMess = "Data Inizio contratto"
        End If
        If DataCalcoloSc.Trim = "" Or Not IsDate(DataCalcoloSc.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", myMess & " obbligatoria per il calcolo Scadenze", WUC_ModalPopup.TYPE_ERROR)
            Calcola_ScadenzeCA = False
            Exit Function
        End If
        '----------------------------------------------
        'giu280120 calcolo delle rate
        NumRate = 0 : NumGG = 0
        If chkNoDivTotRate.Checked = True Then
            If CalcolaNumRate(NumRate, NumGG, strErrore) = False Then
                _WucElement.SetTxtlblMessDoc("Errore: Nella compilazione Scadenze" & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, ""))
                Calcola_ScadenzeCA = False
                Exit Function
            End If
            If NumRate = 0 Then
                Calcola_ScadenzeCA = False
                Dim myErrore As String = "N° Rate non valido - Codice Visita definito nella Tabella Tipo Contratto,<br>Non è presente nel dettaglio Apparechiature.<br>Si prega di inserirlo e rigenerare le Attività Periodo.<br> Oppure Riprovate la modifica uscendo e rientrando e aggiornando (TP4)"
                _WucElement.SetTxtlblMessDoc(myErrore)
                ' ''ModalPopup.Show("Errore: ", myErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        End If
        '---------------------
        If TipoFatt = "U" Then
            NumRate = 1
        End If
        '---------------------
        'ok
        Dim Arr_Giorni(NumRate) As String
        Dim Arr_Scad(NumRate) As String
        Dim Arr_Serie(NumRate) As String
        Dim Arr_Impo(NumRate) As Decimal
        For i = 0 To UBound(Arr_Serie) - 1
            Arr_Serie(i) = ""
        Next i
        Dim myTotNettoPagare As Decimal = CDec(LblTotNettoPagare.Text)
        '---------
        If chkNoDivTotRate.Checked = True Or TipoFatt = "U" Then
            If CompilaScadenzeCA(rowPag, CDate(DataCalcoloSc), myTotNettoPagare, _
              DecimaliValuta, NumRate, NumGG, Arr_Giorni, Arr_Scad, Arr_Impo, TotRate, strErrore) = False Then
                _WucElement.SetTxtlblMessDoc("(CSCA) Errore: Nella compilazione Scadenze: " & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, ""))
                Calcola_ScadenzeCA = False
                Exit Function
            End If
        Else
            If Calcola_ScadenzeCATP4(rowPag, TipoDocumento, DecimaliValuta, NumRate, strErrore) = False Then
                _WucElement.SetTxtlblMessDoc("(CSCATP4) Errore: Nella compilazione Scadenze" & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, ""))
                Calcola_ScadenzeCA = False
                Exit Function
            End If
            Exit Function 'GIU301223
        End If
        '-
        TotRate = 0
        ArrScadPagCA = New ArrayList
        Dim myScadCA As New ScadPagCAEntity
        If chkAccorpaRateAA.Checked = False Or TipoFatt = "U" Then
            For ind = 0 To UBound(Arr_Scad) - 1
                myScadCA = New ScadPagCAEntity
                myScadCA.NRata = (ind + 1).ToString.Trim
                myScadCA.Data = Arr_Scad(ind)
                myScadCA.Importo = FormattaNumero(Arr_Impo(ind), DecimaliValuta)
                If rowPag(0).Item("TipoPagamento") <> 3 Then
                    myScadCA.Evasa = "0" 'prima volta DOPO BISOGNA BLOCCARE IL CONTRATTO E TUTTE LE MODIFICHE DA GESTIONE - ATTIVE SOLO QUELLE A SCADENZA
                Else
                    myScadCA.Evasa = "1"
                End If

                myScadCA.NFC = ""
                myScadCA.DataFC = ""
                myScadCA.Serie = Arr_Serie(ind)
                'giu191223
                myScadCA.ImportoF = "0"
                myScadCA.ImportoR = FormattaNumero(Arr_Impo(ind), DecimaliValuta)
                '---------
                ArrScadPagCA.Add(myScadCA)
                '-
                TotRate = TotRate + Arr_Impo(ind)
            Next ind
        Else
            Dim SaveAnno As String = "" : Dim SaveDataSc As String = ""
            Dim NewImporto As Decimal = 0
            Dim NewNRata As Integer = 0
            For ind = 0 To UBound(Arr_Scad) - 1
                If SaveAnno = "" Then SaveAnno = Year(CDate(Arr_Scad(ind))).ToString.Trim
                If SaveAnno = Year(CDate(Arr_Scad(ind))).ToString.Trim Then
                    NewImporto += Arr_Impo(ind)
                    SaveDataSc = Arr_Scad(ind)
                Else
                    NewNRata += 1
                    myScadCA = New ScadPagCAEntity
                    myScadCA.NRata = NewNRata.ToString.Trim
                    myScadCA.Data = SaveDataSc
                    myScadCA.Importo = FormattaNumero(NewImporto, DecimaliValuta)
                    If rowPag(0).Item("TipoPagamento") <> 3 Then
                        myScadCA.Evasa = "0" 'prima volta DOPO BISOGNA BLOCCARE IL CONTRATTO E TUTTE LE MODIFICHE DA GESTIONE - ATTIVE SOLO QUELLE A SCADENZA
                    Else
                        myScadCA.Evasa = "1"
                    End If

                    myScadCA.NFC = ""
                    myScadCA.DataFC = ""
                    myScadCA.Serie = ""
                    'giu191223
                    myScadCA.ImportoF = "0"
                    myScadCA.ImportoR = FormattaNumero(NewImporto, DecimaliValuta)
                    '---------
                    ArrScadPagCA.Add(myScadCA)
                    '- Prossimo
                    NewImporto = Arr_Impo(ind)
                    SaveAnno = Year(CDate(Arr_Scad(ind))).ToString.Trim
                    SaveDataSc = Arr_Scad(ind)
                End If
                '-
                TotRate = TotRate + Arr_Impo(ind)
            Next ind
            'Ultima
            NewNRata += 1
            myScadCA = New ScadPagCAEntity
            myScadCA.NRata = NewNRata.ToString.Trim
            myScadCA.Data = SaveDataSc
            myScadCA.Importo = FormattaNumero(NewImporto, DecimaliValuta)
            If rowPag(0).Item("TipoPagamento") <> 3 Then
                myScadCA.Evasa = "0" 'prima volta DOPO BISOGNA BLOCCARE IL CONTRATTO E TUTTE LE MODIFICHE DA GESTIONE - ATTIVE SOLO QUELLE A SCADENZA
            Else
                myScadCA.Evasa = "1"
            End If

            myScadCA.NFC = ""
            myScadCA.DataFC = ""
            myScadCA.Serie = ""
            'giu191223
            myScadCA.ImportoF = "0"
            myScadCA.ImportoR = FormattaNumero(NewImporto, DecimaliValuta)
            '---------
            ArrScadPagCA.Add(myScadCA)
            NumRate = NewNRata
        End If
        '-
        Session(CSTSCADPAGCA) = ArrScadPagCA
        GridViewDettCASC.DataSource = ArrScadPagCA
        GridViewDettCASC.DataBind()
        '-
        Dim myTotAbbuono As Decimal = 0
        Try
            myTotAbbuono = CDec(txtAbbuono.Text)
        Catch ex As Exception
            myTotAbbuono = 0
        End Try
        TotRate += myTotAbbuono
        '-
        LblTotaleRate.Text = FormattaNumero(TotRate, DecimaliValuta)
        lblNumRate.Text = "Totale rate: " & FormattaNumero(NumRate)
        '-
        If CDec(LblTotNettoPagare.Text) <> TotRate Then
            LblTotaleRate.BackColor = SEGNALA_KO
            LblTotNettoPagare.BackColor = SEGNALA_KO
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Totale Rate scadenze diverso dal totale netto a pagare.", WUC_ModalPopup.TYPE_ERROR)
            Calcola_ScadenzeCA = False
            Session(CSTNONCOMPLETO) = SWSI
            Exit Function
        Else
            LblTotaleRate.BackColor = SEGNALA_OKLBL
            LblTotNettoPagare.BackColor = SEGNALA_OKLBL
        End If

    End Function
    'giu230320 
    Private Function Calcola_ScadenzeCATP4(ByVal rowPag() As DataRow, ByVal TipoDocumento As String, ByVal DecimaliValuta As Integer, ByRef NumRate As Integer, ByRef strErrore As String) As Boolean
        Calcola_ScadenzeCATP4 = True
        '--------------------------------------------------
        Dim i As Integer = 0
        NumRate = 0
        Dim DsContrattiDettALLAtt As New DSDocumenti
        Dim TotaleRate As Decimal = 0
        Dim myTotImporto As Decimal = 0
        Dim Imposta As Decimal = 0
        Dim myErrore As String = ""
        Dim Arr_Scad(NumRate) As String
        Dim Arr_Impo(NumRate) As Decimal
        Dim Arr_Serie(NumRate) As String
        '-
        Dim RegimeIVA As String = Session(CSTREGIMEIVA)
        If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        '-
        Dim IVACA As Integer = 0
        If Val(RegimeIVA) > 49 Then
            IVACA = Val(RegimeIVA)
        Else
            IVACA = GetParamGestAzi(Session(ESERCIZIO)).IVATrasporto
        End If
        Dim RowDScadS As DSDocumenti.ContrattiDRow
        Try
            Dim myID As String = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                _WucElement.SetTxtlblMessDoc("Errore SESSIONE SCADENZE SCADUTA - IDDOCUMENTO NON VALIDO (Calcola_ScadenzeCATP4)")
                Exit Function
            End If
            Dim pCodVisita As String = rowPag(0).Item("CodVisita").ToString.Trim
            '-
            SetCdmDAdp()
            DsContrattiDettALLAtt.ContrattiD.Clear()
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
            SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = 1 'fisso per le attività per periodo
            SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
            SqlAdapDocDett.Fill(DsContrattiDettALLAtt.ContrattiD)
            'ok NRate
            DVScadAttNRate = New DataView(DsContrattiDettALLAtt.ContrattiD)
            If DVScadAttNRate.Count > 0 Then DVScadAttNRate.Sort = "DataSc,Serie"
            DVScadAttNRate.RowFilter = "Cod_Articolo='" & pCodVisita & "'"
            NumRate = DVScadAttNRate.Count
            If NumRate = 0 Then
                Calcola_ScadenzeCATP4 = False
                lblTotaleAtt.Text = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                myErrore = "N° Rate non valido - Codice Visita definito nella Tabella Tipo Contratto,<br>Non è presente nel dettaglio Apparechiature.<br>Si prega di inserirlo e rigenerare le Attività Periodo.<br> Oppure Riprovate la modifica uscendo e rientrando e aggiornando (TP4)"
                _WucElement.SetTxtlblMessDoc(myErrore)
                Exit Function
            End If
            '-
            ReDim Arr_Scad(NumRate)
            ReDim Arr_Impo(NumRate)
            ReDim Arr_Serie(NumRate)
            TotaleRate = 0
            For i = 0 To DVScadAttNRate.Count - 1
                Arr_Scad(i) = DVScadAttNRate.Item(i).Item("TextDataSc")
                Arr_Serie(i) = IIf(IsDBNull(DVScadAttNRate.Item(i).Item("Serie")), "", DVScadAttNRate.Item(i).Item("Serie").ToString.Trim)
                ' 
                myTotImporto = 0 : Imposta = 0
                Dim RowsScadS() As DataRow = DsContrattiDettALLAtt.ContrattiD.Select("DurataNumRiga=" & DVScadAttNRate.Item(i).Item("DurataNumRiga").ToString.Trim + _
                    " AND Serie='" + Arr_Serie(i) + "'")
                For Each RowDScadS In RowsScadS
                    myTotImporto += RowDScadS.Importo
                Next
                If (IVACA < 50) And (IVACA > 0) And ChkSplitIVA.Checked = False Then
                    Imposta = FormattaNumero((myTotImporto / 100) * (IVACA), DecimaliValuta)
                End If
                Arr_Impo(i) = myTotImporto + Imposta
                TotaleRate += Arr_Impo(i)
            Next
        Catch ex As Exception
            _WucElement.SetTxtlblMessDoc("Errore LETTURA DATI CONTRATTO (Calcola_ScadenzeCATP4)<br>" & ex.Message.Trim)
            Exit Function
        End Try
        '-
        Dim myTotNettoPagare As Decimal = 0
        Try
            myTotNettoPagare = CDec(LblTotNettoPagare.Text)
            If myTotNettoPagare = 0 Then myTotNettoPagare = TotaleRate
            ' non ancora popolato quindi prendo dallo  scadenzario, TotaleRate calcolato dopo il FILL
            '---------
        Catch ex As Exception
            myTotNettoPagare = 0
        End Try
        '-
        Dim myScadCA As New ScadPagCAEntity
        ArrScadPagCA = New ArrayList
        Dim SaveDataSc As String = ""
        For i = 0 To UBound(Arr_Scad) - 1
            'Reggruppato per Periodo: DurataNumRiga
            myScadCA = New ScadPagCAEntity
            myScadCA.NRata = (i + 1).ToString.Trim
            myScadCA.Data = Arr_Scad(i)
            myScadCA.Importo = FormattaNumero(Arr_Impo(i), DecimaliValuta)
            'GIU301223 myScadCA.Evasa = "0"
            If rowPag(0).Item("TipoPagamento") <> 3 Then
                myScadCA.Evasa = "0" 'prima volta DOPO BISOGNA BLOCCARE IL CONTRATTO E TUTTE LE MODIFICHE DA GESTIONE - ATTIVE SOLO QUELLE A SCADENZA
            Else
                myScadCA.Evasa = "1"
            End If
            myScadCA.NFC = ""
            myScadCA.DataFC = ""
            myScadCA.Serie = Arr_Serie(i)
            'giu191223
            myScadCA.ImportoF = "0"
            myScadCA.ImportoR = FormattaNumero(Arr_Impo(i), DecimaliValuta)
            '---------
            ArrScadPagCA.Add(myScadCA)
            '-
        Next i
        '-
        Dim myTotAbbuono As Decimal = 0
        Try
            myTotAbbuono = CDec(txtAbbuono.Text)
        Catch ex As Exception
            myTotAbbuono = 0
        End Try
        TotaleRate += myTotAbbuono
        '-ok ora se devo accorpare per ANNO
        Dim SaveAnno As String = "" : SaveDataSc = ""
        Dim NewImporto As Decimal = 0
        Dim NewNRata As Integer = 0
        Dim SaveSerie As String = ""
        If chkAccorpaRateAA.Checked Or chkNoDivTotRate.Checked Then
            ArrScadPagCA = Nothing
            ArrScadPagCA = New ArrayList
            TotaleRate = 0

            For i = 0 To UBound(Arr_Scad) - 1
                If SaveAnno = "" Then SaveAnno = Year(CDate(Arr_Scad(i))).ToString.Trim
                If SaveAnno = Year(CDate(Arr_Scad(i))).ToString.Trim Then
                    NewImporto += Arr_Impo(i)
                    SaveDataSc = Arr_Scad(i)
                Else
                    NewNRata += 1
                    myScadCA = New ScadPagCAEntity
                    myScadCA.NRata = NewNRata.ToString.Trim
                    myScadCA.Data = SaveDataSc
                    myScadCA.Importo = FormattaNumero(NewImporto, DecimaliValuta)
                    If rowPag(0).Item("TipoPagamento") <> 3 Then
                        myScadCA.Evasa = "0" 'prima volta DOPO BISOGNA BLOCCARE IL CONTRATTO E TUTTE LE MODIFICHE DA GESTIONE - ATTIVE SOLO QUELLE A SCADENZA
                    Else
                        myScadCA.Evasa = "1"
                    End If

                    myScadCA.NFC = ""
                    myScadCA.DataFC = ""
                    myScadCA.Serie = ""
                    'giu191223
                    myScadCA.ImportoF = "0"
                    myScadCA.ImportoR = FormattaNumero(NewImporto, DecimaliValuta)
                    '---------
                    ArrScadPagCA.Add(myScadCA)
                    '- Prossimo
                    NewImporto = Arr_Impo(i)
                    SaveAnno = Year(CDate(Arr_Scad(i))).ToString.Trim
                    SaveDataSc = Arr_Scad(i)
                End If
                '-
                TotaleRate += Arr_Impo(i)
            Next i
            TotaleRate += myTotAbbuono
            'Ultima
            NewNRata += 1
            myScadCA = New ScadPagCAEntity
            myScadCA.NRata = NewNRata.ToString.Trim
            myScadCA.Data = SaveDataSc
            myScadCA.Importo = FormattaNumero(NewImporto, DecimaliValuta)
            If rowPag(0).Item("TipoPagamento") <> 3 Then
                myScadCA.Evasa = "0" 'prima volta DOPO BISOGNA BLOCCARE IL CONTRATTO E TUTTE LE MODIFICHE DA GESTIONE - ATTIVE SOLO QUELLE A SCADENZA
            Else
                myScadCA.Evasa = "1"
            End If

            myScadCA.NFC = ""
            myScadCA.DataFC = ""
            myScadCA.Serie = ""
            'giu191223
            myScadCA.ImportoF = "0"
            myScadCA.ImportoR = FormattaNumero(NewImporto, DecimaliValuta)
            '---------
            ArrScadPagCA.Add(myScadCA)
            NumRate = NewNRata
        
        End If
        'ok
        LblTotaleRate.Text = FormattaNumero(TotaleRate, DecimaliValuta)
        lblNumRate.Text = "Totale rate: " & FormattaNumero(NumRate)
        '-
        Session(CSTSCADPAGCA) = ArrScadPagCA
        GridViewDettCASC.DataSource = ArrScadPagCA
        GridViewDettCASC.DataBind()
        '-
        If myTotNettoPagare <> TotaleRate Then
            LblTotaleRate.BackColor = SEGNALA_KO
            LblTotNettoPagare.BackColor = SEGNALA_KO
            _WucElement.SetTxtlblMessDoc("Attenzione, Totale Rate scadenze diverso dal totale netto a pagare.")
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Totale Rate scadenze diverso dal totale netto a pagare.", WUC_ModalPopup.TYPE_ERROR)
            Calcola_ScadenzeCATP4 = False
            Session(CSTNONCOMPLETO) = SWSI
            Exit Function
        Else
            LblTotaleRate.BackColor = SEGNALA_OKLBL
            LblTotNettoPagare.BackColor = SEGNALA_OKLBL
        End If
    End Function
    Public Function CalcolaNumRate(ByRef NumRate As Integer, ByRef NumGG As Integer, ByVal strErrore As String) As Boolean
        'giu290120
        strErrore = ""
        '-
        Dim NumGGAnno As String = Session(CSTNUMGGANNO)
        Dim strValore As String = ""
        If String.IsNullOrEmpty(NumGGAnno) Then NumGGAnno = ""
        If Not IsNumeric(NumGGAnno) Then NumGGAnno = ""
        If NumGGAnno = "" Or Not IsNumeric(NumGGAnno.Trim) Then
            Call GetDatiAbilitazioni(CSTABILAZI, "NumGGAnno", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If Not IsNumeric(strValore.Trim) Then
                strValore = "365"
            End If
            NumGGAnno = strValore.Trim
        ElseIf CInt(NumGGAnno) = 0 Then
            Call GetDatiAbilitazioni(CSTABILAZI, "NumGGAnno", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If Not IsNumeric(strValore.Trim) Then
                strValore = "365"
            End If
            NumGGAnno = strValore.Trim
        End If
        Session(CSTNUMGGANNO) = NumGGAnno.Trim
        '-
        strErrore = ""
        Dim DurataNum As String = Session(CSTDURATANUM)
        If String.IsNullOrEmpty(DurataNum) Then DurataNum = ""
        If Not IsNumeric(DurataNum) Then DurataNum = ""
        If DurataNum = "" Then
            strErrore = "Manca la Durata N°"
            Return False
        End If
        '-
        Dim DurataTipo As String = Session(CSTDURATATIPO)
        If String.IsNullOrEmpty(DurataTipo) Then DurataTipo = ""
        If DurataTipo = "" Then
            strErrore = "Tipo Durata non definito. "
            Return False
        End If
        '-
        Dim TipoFatt As String = Session(CSTTIPOFATT)
        If String.IsNullOrEmpty(TipoFatt) Then TipoFatt = ""
        If TipoFatt = "" Then
            strErrore = "Tipo Fatturazione non definito. "
            Return False
        ElseIf TipoFatt = "U" Then 'GIU281023
            NumRate = 1
            CalcolaNumRate = True
            lblNumRate.Text = "Totale rate: " & FormattaNumero(NumRate)
            Exit Function
        End If
        '-
        NumRate = 0
        NumGG = 0
        strErrore = ""
        CalcolaNumRate = True
        If DurataTipo.Trim = "A" Then
            NumGG = CInt(DurataNum) * CInt(NumGGAnno)
        ElseIf DurataTipo.Trim = "M" Then
            NumGG = CInt(DurataNum) * 30
        ElseIf DurataTipo.Trim = "T" Then
            NumGG = CInt(DurataNum) * 90
        ElseIf DurataTipo.Trim = "Q" Then
            NumGG = CInt(DurataNum) * 120
        ElseIf DurataTipo.Trim = "S" Then
            NumGG = CInt(DurataNum) * 180
        Else
            strErrore = "Manca il Tipo Durata N°"
            Return False
        End If
        If TipoFatt.Trim = "A" Then
            NumRate = NumGG / CInt(NumGGAnno)
            NumGG = CInt(NumGGAnno)
        ElseIf TipoFatt.Trim = "M" Then
            NumRate = NumGG / 30
            NumGG = 30
        ElseIf TipoFatt.Trim = "T" Then
            NumRate = NumGG / 90
            NumGG = 90
        ElseIf TipoFatt.Trim = "Q" Then
            NumRate = NumGG / 120
            NumGG = 120
        ElseIf TipoFatt.Trim = "S" Then
            NumRate = NumGG / 180
            NumGG = 180
        Else
            strErrore = "Manca il Tipo Fatturazione"
            Return False
        End If
        CalcolaNumRate = True
        lblNumRate.Text = "Totale rate: " & FormattaNumero(NumRate)
    End Function

    Private Sub ChkSplitIVA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkSplitIVA.CheckedChanged
        'GIU180219
        Dim myCodCli As String = Session(COD_CLIENTE)
        If IsNothing(myCodCli) Then
            myCodCli = ""
        End If
        If String.IsNullOrEmpty(myCodCli) Then
            myCodCli = ""
        End If
        ChkSplitIVA.BackColor = SEGNALA_OK
        Dim strErrore As String = "" : Dim mySplitIVA As Boolean = False
        If myCodCli.Trim <> "" Then
            Call Documenti.CKClientiIPAByIDConORCod(0, myCodCli.Trim, mySplitIVA, strErrore)
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ALERT)
                ' ''Exit Function
            ElseIf ChkSplitIVA.Checked = True And mySplitIVA = False Then
                ChkSplitIVA.AutoPostBack = False
                ChkSplitIVA.Checked = mySplitIVA
                ChkSplitIVA.AutoPostBack = True
                ChkSplitIVA.BackColor = SEGNALA_INFO
                strErrore = "Attenzione, il cliente selezionato non ha lo Split payment IVA"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ALERT)
            ElseIf ChkSplitIVA.Checked = False And mySplitIVA = True Then
                ChkSplitIVA.AutoPostBack = False
                ChkSplitIVA.Checked = mySplitIVA
                ChkSplitIVA.AutoPostBack = True
                ChkSplitIVA.BackColor = SEGNALA_INFO
                strErrore = "Attenzione, il cliente selezionato ha lo Split payment IVA"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ALERT)
            End If

        End If
        '-----------------------------------------------------------------
        Call TotaleNettoPag()
        Session(SWMODIFICATO) = SWSI
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        Dim DecimaliValutaFinito As Integer = Int(DecimaliVal)
        '------------------------------------
        myTipoDoc = Session(CSTTIPODOC)
        If IsNothing(myTipoDoc) Then
            myTipoDoc = ""
        End If
        If String.IsNullOrEmpty(myTipoDoc) Then
            myTipoDoc = ""
        End If
        If myTipoDoc = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore: TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '---------------------------------------------------------------
        If Calcola_ScadenzeCA(myTipoDoc, DecimaliValutaFinito, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Sub
        End If
    End Sub
    'GIU080118
    Private Sub TotaleNettoPag()
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        '------------------------------------
        If ChkSplitIVA.Checked = True Then
            LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotaleImpon.Text), Int(DecimaliVal))
        Else
            LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotDocumento.Text), Int(DecimaliVal))
        End If
        '-
        'If chkRitAcconto.Checked = True And txtImponibileRA.Text <> "" And txtPercRA.Text <> "" Then
        '    Dim myImp As Decimal = 0 : Dim myPerc As Decimal = 0
        '    Try
        '        myImp = CDec(txtImponibileRA.Text)
        '    Catch ex As Exception
        '        myImp = 0
        '    End Try
        '    Try
        '        myPerc = CDec(txtPercRA.Text)
        '    Catch ex As Exception
        '        myPerc = 0
        '    End Try
        '    If myImp <> 0 And myPerc <> 0 Then
        '        LblTotaleRA.Text = FormattaNumero(((myImp / 100) * myPerc), Int(DecimaliVal))
        '    Else
        '        LblTotaleRA.Text = ""
        '    End If
        '    '-
        'Else
        '    LblTotaleRA.Text = ""
        'End If
        '-
        Dim myTot As Decimal = 0
        'Try
        '    myTot = CDec(LblTotaleRA.Text)
        'Catch ex As Exception
        '    myTot = 0
        'End Try
        'If chkRitAcconto.Checked = True And myTot <> 0 Then
        '    LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotNettoPagare.Text) - myTot, Int(DecimaliVal))
        'End If
        Try 'giu150320
            myTot = CDec(txtAbbuono.Text)
        Catch ex As Exception
            myTot = 0
        End Try
        LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotNettoPagare.Text) + myTot, Int(DecimaliVal))
        '-
    End Sub

    Public Function SetChkSplitIVA(ByVal _SplitIVA As Boolean, ByRef strErrore As String) As Boolean
        ChkSplitIVA.AutoPostBack = False
        ChkSplitIVA.Checked = _SplitIVA
        ChkSplitIVA.AutoPostBack = True
        SetErrChkSplitIVA(True)
    End Function
    'giu010319
    Public Function SetErrChkSplitIVA(ByVal _OK As Boolean) As Boolean
        If _OK = False Then
            ChkSplitIVA.BackColor = SEGNALA_KO
        Else
            ChkSplitIVA.BackColor = SEGNALA_OK
        End If
    End Function
    Public Function SetErrBollo(ByVal _OK As Boolean) As Boolean
        If _OK = False Then
            txtBollo.BackColor = SEGNALA_KO
        Else
            txtBollo.BackColor = SEGNALA_OK
        End If
    End Function
    'Public Function SetErrRitAcc(ByVal _OK As Boolean) As Boolean
    '    If _OK = False Then
    '        txtImponibileRA.BackColor = SEGNALA_KO
    '        txtPercRA.BackColor = SEGNALA_KO
    '        chkRitAcconto.BackColor = SEGNALA_KO
    '    Else
    '        txtImponibileRA.BackColor = SEGNALA_OK
    '        txtPercRA.BackColor = SEGNALA_OK
    '        chkRitAcconto.BackColor = SEGNALA_OK
    '    End If
    'End Function

    Private Sub DDLSpeseBollo_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLSpeseBollo.SelectedIndexChanged
        If DDLSpeseBollo.SelectedValue.Trim <> "" Then
            Try
                If txtBollo.Text.Trim = "" Then
                    txtBollo.Text = "0,00"
                End If
                If CDec(txtBollo.Text.Trim) = 0 Then
                    txtBollo.Text = GetParamGestAzi(Session(ESERCIZIO)).Bollo
                    txtBollo.Text = FormattaNumero(CDec(txtBollo.Text), 2)
                End If
            Catch ex As Exception
                txtBollo.Text = "0,00"
            End Try
            If CDec(txtBollo.Text.Trim) = 0 Then
                txtBollo.Text = GetParamGestAzi(Session(ESERCIZIO)).Bollo
                txtBollo.Text = FormattaNumero(CDec(txtBollo.Text), 2)
            End If
        Else
            txtBollo.Text = "0,00"
        End If
        txtBollo.BackColor = SEGNALA_OK
    End Sub

    Public Function SetLblTotLMPL(ByVal _TotLM As Decimal, ByVal _Deduzioni As Decimal) As Boolean
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = "2"
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        '------------------------------------
        lblTotLordoMerce.Text = FormattaNumero(_TotLM, Int(DecimaliVal))
        lblTotDeduzioni.Text = FormattaNumero(_Deduzioni, Int(DecimaliVal))
    End Function

    Private Sub TxtScontoCassa_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtScontoCassa.TextChanged
        TxtScontoCassa.Text = TxtScontoCassa.Text.Replace(".", ",")
        TxtScontoCassa.Text = IIf(Not IsNumeric(TxtScontoCassa.Text.Trim), 0, TxtScontoCassa.Text.Trim)
        TxtScontoCassa.Text = FormattaNumero(CDec(TxtScontoCassa.Text), 2)
        Session(CSTSCCASSA) = TxtScontoCassa.Text.Trim
        Session(SWMODIFICATO) = SWSI
        Dim myErrore As String = ""
        If _WucElement.AggImportiTot(myErrore) = False Then
            'messaggio segnalato da WUC_Contratti _WucElement.Chiudi("Errore: Aggiornamento importi e scadenze")
            Exit Sub
        End If
        TxtSpeseIncasso.Focus()
    End Sub
#Region "SCADENZE FATTURAZIONE CONTRATTO"

    Private Sub GridViewDettCASC_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDettCASC.RowDataBound
        Try
            If e.Row.DataItemIndex > -1 Then
                If IsNumeric(e.Row.Cells(CellCASC.Importo).Text) Then
                    If CDec(e.Row.Cells(CellCASC.Importo).Text) <> 0 Then
                        e.Row.Cells(CellCASC.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellCASC.Importo).Text), 2).ToString
                    Else
                        e.Row.Cells(CellCASC.Importo).Text = ""
                    End If
                End If
                If IsDate(e.Row.Cells(CellCASC.Data).Text) Then
                    e.Row.Cells(CellCASC.Data).Text = Format(CDate(e.Row.Cells(CellCASC.Data).Text), FormatoData).ToString
                ElseIf e.Row.Cells(CellCASC.Data).Text.Trim <> "" Then
                    e.Row.Cells(CellCASC.Data).Text = e.Row.Cells(CellCASC.Data).BackColor = SEGNALA_KO
                End If
                If IsDate(e.Row.Cells(CellCASC.DataFC).Text) Then
                    e.Row.Cells(CellCASC.DataFC).Text = Format(CDate(e.Row.Cells(CellCASC.DataFC).Text), FormatoData).ToString
                ElseIf e.Row.Cells(CellCASC.DataFC).Text.Trim <> "" Then
                    e.Row.Cells(CellCASC.DataFC).Text = e.Row.Cells(CellCASC.DataFC).BackColor = SEGNALA_KO
                End If
                If IsNumeric(e.Row.Cells(CellCASC.ImportoF).Text) Then
                    If CDec(e.Row.Cells(CellCASC.ImportoF).Text) <> 0 Then
                        e.Row.Cells(CellCASC.ImportoF).Text = FormattaNumero(CDec(e.Row.Cells(CellCASC.ImportoF).Text), 2).ToString
                    Else
                        e.Row.Cells(CellCASC.ImportoF).Text = ""
                    End If
                End If
                If IsNumeric(e.Row.Cells(CellCASC.ImportoR).Text) Then
                    If CDec(e.Row.Cells(CellCASC.ImportoR).Text) <> 0 Then
                        e.Row.Cells(CellCASC.ImportoR).Text = FormattaNumero(CDec(e.Row.Cells(CellCASC.ImportoR).Text), 2).ToString
                    Else
                        e.Row.Cells(CellCASC.ImportoR).Text = ""
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    'NON VA BENE SE DEVO FARE LE MODIFICHE - 
    '''Private Sub GridViewDettCAAtt_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewDettCAAtt.PageIndexChanging
    '''    If (e.NewPageIndex = -1) Then
    '''        GridViewDettCAAtt.PageIndex = 0
    '''    Else
    '''        GridViewDettCAAtt.PageIndex = e.NewPageIndex
    '''    End If
    '''    GridViewDettCAAtt.SelectedIndex = -1
    '''    DVScadAtt = Session(CSTSCADATTCA)
    '''    '-
    '''    GridViewDettCAAtt.DataSource = DVScadAtt
    '''    GridViewDettCAAtt.DataBind()
    '''End Sub
    Private Sub btnModScPagCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModScPagCA.Click
        Session(CALLGESTIONE) = SWNO
        lblMessRatePag.Text = "" ': lblMessRatePag.Visible = False
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If GridViewDettCASC.Rows.Count > 0 Then
            'OK
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato presente.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODSCATT
        Session(SWOPMODSCATT) = SWSI
        GridViewDettCASC.Enabled = True
        btnModScPagCA.Visible = False
        btnAggScPagCA.Visible = True
        btnAnnScPagCA.Visible = True
        btnAggFCFromOC.Enabled = False
        btnAggDataScAtt.Enabled = False
       
    End Sub
    Private Sub btnModScAttCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModScAttCA.Click
        Session(CALLGESTIONE) = SWNO
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If GridViewDettCAAtt.Rows.Count > 0 Then
            'OK
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato presente.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session(CSTSCADATTCA)
            DVScadAttSAVE = New DataView(DVScadAtt.ToTable)
            Session("DVScadAttSAVE") = DVScadAttSAVE
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODSCATT
        Session(SWOPMODSCATT) = SWSI
        GridViewDettCAAtt.Enabled = True
        btnModScAttCA.Visible = False
        btnAggScAttCA.Visible = True
        btnAnnScAttCA.Visible = True
        btnAggFCFromOC.Enabled = False
        btnAggDataScAtt.Enabled = False
    End Sub

    Private Sub btnAnnScPagCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnScPagCA.Click
        Session(CALLGESTIONE) = SWNO
        lblMessRatePag.Text = "" ': lblMessRatePag.Visible = False
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        GridViewDettCASC.Enabled = False
        btnModScPagCA.Visible = True
        btnAggScPagCA.Visible = False
        btnAnnScPagCA.Visible = False
        btnAggFCFromOC.Enabled = True
        btnAggDataScAtt.Enabled = True
        ArrScadPagCA = New ArrayList
        If Not (Session(CSTSCADPAGCA) Is Nothing) Then
            ArrScadPagCA = Session(CSTSCADPAGCA)
            GridViewDettCASC.DataSource = ArrScadPagCA
            GridViewDettCASC.DataBind()
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

    End Sub
    Private Sub btnAnnScAttCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnScAttCA.Click
        Session(CALLGESTIONE) = SWNO
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        GridViewDettCAAtt.Enabled = False
        btnModScAttCA.Visible = True
        btnAggScAttCA.Visible = False
        btnAnnScAttCA.Visible = False
        btnAggFCFromOC.Enabled = True
        btnAggDataScAtt.Enabled = True
        If Not (Session("DVScadAttSAVE") Is Nothing) Then 'If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session("DVScadAttSAVE") 'Session(CSTSCADATTCA)
            Session(CSTSCADATTCA) = DVScadAtt
            lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
            DVScadAtt.RowFilter = "Qta_Evasa<>0"
            lblTotaleAtt.Text += " di cui Evase: " & DVScadAtt.Count.ToString.Trim
            '-
            DVScadAtt.RowFilter = "Qta_Fatturata<>0"
            lblTotaleAtt.Text += " Fatturate: " & DVScadAtt.Count.ToString.Trim
            '-
            DVScadAtt.RowFilter = ""
            If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,Serie"
            GridViewDettCAAtt.DataSource = DVScadAtt
            GridViewDettCAAtt.DataBind()
            Call AnnullaALLScadAtt()
            Call _WucElement.ReBuildDett()
            Call VerificaDateCKCons("")
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

    End Sub
    Private Sub btnAggScAttCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggScAttCA.Click
        Session(CALLGESTIONE) = SWNO
        If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session(CSTSCADATTCA)
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(CALLGESTIONE) = SWNO
        'controllo date scadenze sempre valide per tutte
        Dim SWOKEvasa As Boolean = False
        Dim idxRow As Integer = 0
        Dim chkEvasa As CheckBox
        Dim chkFatturata As CheckBox 'giu251123
        Dim txtDSC As TextBox
        Dim txtDEV As TextBox
        Dim txtDSCNext As TextBox
        Dim SWErrore As Boolean = False
        Dim strErrore As String = ""
        Dim Comodo As String = ""
        idxRow = 0
        Dim idxRowDV As Integer = 0
        Dim SWAgg As Boolean = False
        Try
            For Each row As GridViewRow In GridViewDettCAAtt.Rows
                SWErrore = False
                chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
                txtDSC = CType(row.FindControl("txtDataSC"), TextBox)
                txtDEV = CType(row.FindControl("txtDataEv"), TextBox)
                chkFatturata = CType(row.FindControl("chkFatturata"), CheckBox)
                '-
                If txtDSC.Text.Trim.Length <> 10 Then
                    txtDSC.BackColor = SEGNALA_KO
                    SWErrore = True
                ElseIf Not IsDate(txtDSC.Text.Trim) Then
                    txtDSC.BackColor = SEGNALA_KO
                    SWErrore = True
                Else
                    txtDSC.BackColor = SEGNALA_OK
                End If
                '-
                If txtDEV.Text.Trim = "" Then
                    txtDEV.BackColor = SEGNALA_OK
                ElseIf txtDEV.Text.Trim.Length <> 10 Then
                    txtDEV.BackColor = SEGNALA_KO
                    SWErrore = True
                ElseIf Not IsDate(txtDEV.Text.Trim) Then
                    txtDEV.BackColor = SEGNALA_KO
                    SWErrore = True
                End If
                'giu080223
                txtDSCNext = CType(row.FindControl("txtDataEVN"), TextBox)
                If txtDSCNext.Text.Trim <> "" Then
                    If Not IsDate(txtDSCNext.Text.Trim) Then
                        txtDSCNext.BackColor = SEGNALA_KO
                        SWErrore = True
                    Else
                        txtDSCNext.BackColor = SEGNALA_OK
                    End If
                Else
                    txtDSCNext.BackColor = SEGNALA_OK
                End If
                If Not SWErrore Then
                    idxRowDV = idxRow + (GridViewDettCAAtt.PageSize * GridViewDettCAAtt.PageIndex)
                    If chkEvasa.Checked Then
                        If DVScadAtt(idxRowDV).Item("Qta_Evasa") = 0 Or txtDEV.Text.Trim = "" Then
                            SWAgg = True
                        End If
                        If IsDBNull(DVScadAtt.Item(idxRowDV).Item("DataEv")) Then
                            SWAgg = True
                        ElseIf DVScadAtt.Item(idxRowDV).Item("DataEv").ToString.Trim = "" Then
                            SWAgg = True
                        End If
                    ElseIf DVScadAtt(idxRowDV).Item("Qta_Evasa") <> 0 Or txtDEV.Text.Trim <> "" Then
                        SWAgg = True
                    Else
                        If IsDBNull(DVScadAtt.Item(idxRowDV).Item("DataEv")) Then
                            'ok
                        ElseIf DVScadAtt.Item(idxRowDV).Item("DataEv").ToString.Trim <> "" Then
                            SWAgg = True
                        End If
                    End If
                    If chkFatturata.Checked Then
                        If DVScadAtt(idxRowDV).Item("Qta_Fatturata") = 0 Then
                            SWAgg = True
                        End If
                    ElseIf DVScadAtt(idxRowDV).Item("Qta_Fatturata") <> 0 Then
                        SWAgg = True
                    End If
                    If DVScadAtt(idxRowDV).Item("TextDataSc").ToString.Trim <> txtDSC.Text.Trim Then
                        SWAgg = True
                    End If
                    If IsDBNull(DVScadAtt(idxRowDV).Item("TextDataEv")) Then
                        Comodo = ""
                    Else
                        Comodo = DVScadAtt(idxRowDV).Item("TextDataEv").ToString.Trim
                    End If
                    If Comodo.Trim <> txtDEV.Text.Trim Then
                        SWAgg = True
                    End If
                    If IsDBNull(DVScadAtt(idxRowDV).Item("TextRefDataNC")) Then
                        Comodo = ""
                    Else
                        Comodo = DVScadAtt(idxRowDV).Item("TextRefDataNC").ToString.Trim
                    End If
                    If txtDSCNext.Text.Trim <> Comodo.Trim Then
                        SWAgg = True
                    End If
                End If
                idxRow += 1
            Next
        Catch ex As Exception
            SWErrore = True
            strErrore = ex.Message.Trim
        End Try
        If SWErrore Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiornamento Attività: ", "Data scadenza errata.<br>" + strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-------------------------------
        'OK Riporto le modifiche
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
        GridViewDettCAAtt.Enabled = False
        btnModScAttCA.Visible = True
        btnAggScAttCA.Visible = False
        btnAnnScAttCA.Visible = False
        btnAggFCFromOC.Enabled = True
        btnAggDataScAtt.Enabled = True
        If SWAgg = False Then
            Call VerificaDateCKCons("")
            lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
            DVScadAtt.RowFilter = "Qta_Evasa<>0"
            lblTotaleAtt.Text += " di cui Evase: " & DVScadAtt.Count.ToString.Trim
            '-
            DVScadAtt.RowFilter = "Qta_Fatturata<>0"
            lblTotaleAtt.Text += " Fatturate: " & DVScadAtt.Count.ToString.Trim
            '-
            DVScadAtt.RowFilter = ""
            If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,Serie"
            GridViewDettCAAtt.DataSource = DVScadAtt
            GridViewDettCAAtt.DataBind()
            Exit Sub 'NESSUN AGGIORNAMENTO
        End If
        '-------------------------------
        strErrore = ""
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
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataEV", System.Data.SqlDbType.DateTime, 8, "DataEV"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 8, "RefDataNC"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Fatturata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Fatturata", System.Data.DataRowVersion.Current, Nothing))
        If SqlConn.State <> ConnectionState.Open Then
            SqlConn.Open()
        End If
        '---------------------------
        idxRow = 0
        Dim SWOKAgg As Boolean = False
        For Each row As GridViewRow In GridViewDettCAAtt.Rows
            SWErrore = False
            chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
            chkFatturata = CType(row.FindControl("chkFatturata"), CheckBox)
            txtDSC = CType(row.FindControl("txtDataSC"), TextBox)
            txtDEV = CType(row.FindControl("txtDataEv"), TextBox)
            If txtDSC.Text.Trim.Length <> 10 Then
                txtDSC.BackColor = SEGNALA_KO
                SWErrore = True
            ElseIf Not IsDate(txtDSC.Text.Trim) Then
                txtDSC.BackColor = SEGNALA_KO
                SWErrore = True
            End If
            '-
            If txtDEV.Text.Trim = "" Then
                txtDEV.BackColor = SEGNALA_OK
            ElseIf txtDEV.Text.Trim.Length <> 10 Then
                txtDEV.BackColor = SEGNALA_KO
                SWErrore = True
            ElseIf Not IsDate(txtDEV.Text.Trim) Then
                txtDEV.BackColor = SEGNALA_KO
                SWErrore = True
            End If
            'giu080223
            txtDSCNext = CType(row.FindControl("txtDataEVN"), TextBox)
            If txtDSCNext.Text.Trim <> "" Then
                If Not IsDate(txtDSCNext.Text.Trim) Then
                    txtDSCNext.BackColor = SEGNALA_KO
                    SWErrore = True
                Else
                    txtDSCNext.BackColor = SEGNALA_OK
                End If
            Else
                txtDSCNext.BackColor = SEGNALA_OK
            End If
            If SWErrore Then
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                Session(SWOP) = SWOPMODSCATT
                Session(SWOPMODSCATT) = SWSI
                GridViewDettCAAtt.Enabled = True
                btnModScAttCA.Visible = False
                btnAggScAttCA.Visible = True
                btnAnnScAttCA.Visible = True
                btnAggDataScAtt.Enabled = False
                btnAggFCFromOC.Enabled = False
                '-
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", "Data scadenza errata.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            idxRowDV = idxRow + (GridViewDettCAAtt.PageSize * GridViewDettCAAtt.PageIndex)
            'giu031123 solo se ho modificato 
            SWAgg = False
            If chkEvasa.Checked Then
                If DVScadAtt(idxRowDV).Item("Qta_Evasa") = 0 Or txtDEV.Text.Trim = "" Then
                    SWAgg = True
                End If
                If IsDBNull(DVScadAtt.Item(idxRowDV).Item("DataEv")) Then
                    SWAgg = True
                ElseIf DVScadAtt.Item(idxRowDV).Item("DataEv").ToString.Trim = "" Then
                    SWAgg = True
                End If
            ElseIf DVScadAtt(idxRowDV).Item("Qta_Evasa") <> 0 Or txtDEV.Text.Trim <> "" Then
                SWAgg = True
                If IsDBNull(DVScadAtt.Item(idxRowDV).Item("DataEv")) Then
                    'ok
                ElseIf DVScadAtt.Item(idxRowDV).Item("DataEv").ToString.Trim <> "" Then
                    SWAgg = True
                End If
            End If
            If chkFatturata.Checked Then
                If DVScadAtt(idxRowDV).Item("Qta_Fatturata") = 0 Then
                    SWAgg = True
                End If
            ElseIf DVScadAtt(idxRowDV).Item("Qta_Fatturata") <> 0 Then
                SWAgg = True
            End If
            If DVScadAtt(idxRowDV).Item("TextDataSc").ToString.Trim <> txtDSC.Text.Trim Then
                SWAgg = True
            End If
            If IsDBNull(DVScadAtt(idxRowDV).Item("TextDataEv")) Then
                Comodo = ""
            Else
                Comodo = DVScadAtt(idxRowDV).Item("TextDataEv").ToString.Trim
            End If
            If Comodo.Trim <> txtDEV.Text.Trim Then
                SWAgg = True
            End If
            If IsDBNull(DVScadAtt(idxRowDV).Item("TextRefDataNC")) Then
                Comodo = ""
            Else
                Comodo = DVScadAtt(idxRowDV).Item("TextRefDataNC").ToString.Trim
            End If
            If txtDSCNext.Text.Trim <> Comodo.Trim Then
                SWAgg = True
            End If
            If SWAgg = False Then
                idxRow += 1
                Continue For
            End If
            SWOKAgg = True
            '-------------------------------
            DVScadAtt.BeginInit()
            If chkEvasa.Checked Then
                DVScadAtt.Item(idxRowDV).Item("Qta_Selezionata") = DVScadAtt.Item(idxRowDV).Item("Qta_Ordinata")
                DVScadAtt.Item(idxRowDV).Item("Qta_Evasa") = DVScadAtt.Item(idxRowDV).Item("Qta_Ordinata")
                DVScadAtt.Item(idxRowDV).Item("Qta_Residua") = 0
                If txtDEV.Text.Trim = "" Then txtDEV.Text = Format(Now.Date, FormatoData)
            Else
                DVScadAtt.Item(idxRowDV).Item("Qta_Selezionata") = 0
                DVScadAtt.Item(idxRowDV).Item("Qta_Evasa") = 0
                DVScadAtt.Item(idxRowDV).Item("Qta_Residua") = DVScadAtt.Item(idxRowDV).Item("Qta_Ordinata")
                txtDEV.Text = ""
            End If
            DVScadAtt.Item(idxRowDV).Item("TextDataSc") = txtDSC.Text.Trim
            DVScadAtt.Item(idxRowDV).Item("TextDataEv") = txtDEV.Text.Trim
            DVScadAtt.Item(idxRowDV).Item("TextRefDataNC") = txtDSCNext.Text.Trim
            If DVScadAtt.Item(idxRowDV).Item("Qta_Evasa") <> 0 Then SWOKEvasa = True
            If chkFatturata.Checked Then
                DVScadAtt.Item(idxRowDV).Item("Qta_Fatturata") = DVScadAtt.Item(idxRowDV).Item("Qta_Ordinata")
            Else
                DVScadAtt.Item(idxRowDV).Item("Qta_Fatturata") = 0
            End If
            DVScadAtt.EndInit()
            Try
                SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = DVScadAtt.Item(idxRowDV).Item("IDDocumenti")
                SqlUpd_ConTScadPag.Parameters("@DurataNum").Value = DVScadAtt.Item(idxRowDV).Item("DurataNum")
                SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = DVScadAtt.Item(idxRowDV).Item("DurataNumRiga")
                SqlUpd_ConTScadPag.Parameters("@Riga").Value = DVScadAtt.Item(idxRowDV).Item("Riga")
                SqlUpd_ConTScadPag.Parameters("@Qta_Evasa").Value = DVScadAtt.Item(idxRowDV).Item("Qta_Evasa")
                SqlUpd_ConTScadPag.Parameters("@Qta_Residua").Value = DVScadAtt.Item(idxRowDV).Item("Qta_Residua")
                SqlUpd_ConTScadPag.Parameters("@DataSC").Value = CDate(DVScadAtt.Item(idxRowDV).Item("TextDataSc"))
                If IsDBNull(DVScadAtt.Item(idxRowDV).Item("TextDataEv")) Then
                    SqlUpd_ConTScadPag.Parameters("@DataEV").Value = DBNull.Value
                ElseIf DVScadAtt.Item(idxRowDV).Item("TextDataEv").ToString.Trim = "" Or Not IsDate(DVScadAtt.Item(idxRowDV).Item("TextDataEv").ToString.Trim) Then
                    SqlUpd_ConTScadPag.Parameters("@DataEv").Value = DBNull.Value
                Else
                    SqlUpd_ConTScadPag.Parameters("@DataEv").Value = CDate(DVScadAtt.Item(idxRowDV).Item("TextDataEv").ToString.Trim)
                End If
                'giu080223
                If IsDBNull(DVScadAtt.Item(idxRowDV).Item("TextRefDataNC")) Then
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                ElseIf DVScadAtt.Item(idxRowDV).Item("TextRefDataNC").ToString.Trim = "" Or Not IsDate(DVScadAtt.Item(idxRowDV).Item("TextRefDataNC").ToString.Trim) Then
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                Else
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = CDate(DVScadAtt.Item(idxRowDV).Item("TextRefDataNC").ToString.Trim)
                End If
                SqlUpd_ConTScadPag.Parameters("@Qta_Fatturata").Value = DVScadAtt.Item(idxRowDV).Item("Qta_Fatturata")
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
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Catch ex As Exception
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                strErrore = ex.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Finally
                'nulla
            End Try
            idxRow += 1
        Next
        If SWOKAgg = False Then
            Call VerificaDateCKCons("")
            lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
            DVScadAtt.RowFilter = "Qta_Evasa<>0"
            lblTotaleAtt.Text += " di cui Evase: " & DVScadAtt.Count.ToString.Trim
            '-
            DVScadAtt.RowFilter = "Qta_Fatturata<>0"
            lblTotaleAtt.Text += " Fatturate: " & DVScadAtt.Count.ToString.Trim
            '-
            DVScadAtt.RowFilter = ""
            If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,Serie"
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - IDDOCUMENTO NON VALIDO", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        SetCdmDAdp()
        Dim pCodVisita As String = "" : Dim pNVisite As Integer = 0
        If GetCodVisitaDATipoCAIdPag(pCodVisita, pNVisite) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando. Definizione Tipo Contratto errato.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim DsContrattiDettALLAtt As New DSDocumenti
        DsContrattiDettALLAtt.ContrattiD.Clear()
        SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = CLng(myID)
        SqlDbSelectCmd.Parameters.Item("@DurataNum").Value = 1 'fisso per le attività per periodo
        SqlDbSelectCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
        SqlAdapDocDett.Fill(DsContrattiDettALLAtt.ContrattiD)
        DVScadAtt = New DataView(DsContrattiDettALLAtt.ContrattiD)
        Dim RowD As DSDocumenti.ContrattiDRow
        If DVScadAtt.Count > 0 Then
            Dim RowsEvasa() As DataRow = DsContrattiDettALLAtt.ContrattiD.Select("")
            For Each RowD In RowsEvasa
                RowD.BeginEdit()
                RowD.Qta_Selezionata = RowD.Qta_Evasa
                If pCodVisita.Trim.ToUpper = RowD.Cod_Articolo.Trim.ToUpper Then
                    RowD.SWModAgenti = False
                Else
                    RowD.SWModAgenti = True
                End If
                RowD.EndEdit()
            Next
            DsContrattiDettALLAtt.AcceptChanges()
        End If
        '-
        Session(CSTSCADATTCA) = DVScadAtt
        lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
        DVScadAtt.RowFilter = "Qta_Evasa<>0"
        lblTotaleAtt.Text += " di cui Evase: " & DVScadAtt.Count.ToString.Trim
        '-
        DVScadAtt.RowFilter = "Qta_Fatturata<>0"
        lblTotaleAtt.Text += " Fatturate: " & DVScadAtt.Count.ToString.Trim
        '-
        DVScadAtt.RowFilter = ""
        If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,Serie"
        GridViewDettCAAtt.DataSource = DVScadAtt
        GridViewDettCAAtt.DataBind()

        If Not IsNothing(SqlConn) Then
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
                SqlConn = Nothing
            End If
        End If
        If Me.UpgStatoDoc(SWOKEvasa, False, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiornamento StatoDoc: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
        End If
        '''Call _WucElement.TD1ReBuildDett()
        Call _WucElement.ReBuildDett()
        Call _WucElement.CKScAttBollo()
        Call VerificaDateCKCons("")
        '-
    End Sub

    Private Sub btnAggScPagCA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggScPagCA.Click
        lblMessRatePag.Text = "" ': lblMessRatePag.Visible = False
        Session(CALLGESTIONE) = SWNO
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPMODSCATT) = SWOPNESSUNA
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
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA (ID) - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        ArrScadPagCA = New ArrayList
        If Not (Session(CSTSCADPAGCA) Is Nothing) Then
            ArrScadPagCA = Session(CSTSCADPAGCA)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK Riporto le modifiche
        Dim SWOKPagScad As Boolean = False
        Dim rowScadPagCa As ScadPagCAEntity = Nothing
        Dim idxRow As Integer = 0
        Dim chkEvasa As CheckBox
        Dim txtNFC As TextBox
        Dim txtDFC As TextBox
        Dim Comodo As String = ""
        'GIU240821 NEL CASO VENGA INSERITA UNA FATTURA IMPOSTO A TRUE chkEvasa
        For Each row As GridViewRow In GridViewDettCASC.Rows
            chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
            txtNFC = CType(row.FindControl("txtNFC"), TextBox)
            txtDFC = CType(row.FindControl("txtDataFC"), TextBox)
            rowScadPagCa = ArrScadPagCA(idxRow)
            rowScadPagCa.Evasa = chkEvasa.Checked
            If chkEvasa.Checked = True Then
                SWOKPagScad = True
            End If
            If Not (txtNFC Is Nothing) Then
                'GIU191223
                Comodo = txtNFC.Text.Trim.Replace(";", ",")
                txtNFC.Text = NormalizzaStringa(Comodo.Trim)
                '--------
                rowScadPagCa.NFC = NormalizzaStringa(txtNFC.Text.Trim)
                txtNFC.BackColor = SEGNALA_OK
                'giu251123 non piu questa regola per le fatture parziali fino al raggiungimento del totale rata
                If txtNFC.Text.Trim <> "" Then 'GIU240821 NEL CASO VENGA INSERITA UNA FATTURA IMPOSTO A TRUE chkEvasa
                    '''chkEvasa.Checked = True
                    SWOKPagScad = True
                End If
            Else
                rowScadPagCa.NFC = ""
                txtNFC.BackColor = SEGNALA_KO
            End If
            If Not (txtDFC Is Nothing) Then
                If txtDFC.Text.Trim <> "" Then
                    'GIU191223
                    Comodo = txtDFC.Text.Trim.Replace(";", ",")
                    txtDFC.Text = NormalizzaStringa(Comodo.Trim)
                    '--------
                    If IsDate(txtDFC.Text) Then
                        rowScadPagCa.DataFC = NormalizzaStringa(txtDFC.Text.Trim)
                        txtDFC.BackColor = SEGNALA_OK
                    Else
                        rowScadPagCa.DataFC = NormalizzaStringa(txtDFC.Text.Trim)
                        txtDFC.BackColor = SEGNALA_KO
                    End If
                    'giu251123 non piu questa regola per le fatture parziali fino al raggiungimento del totale rata
                    ''''GIU240821 NEL CASO VENGA INSERITA UNA FATTURA IMPOSTO A TRUE chkEvasa
                    '''chkEvasa.Checked = True
                    SWOKPagScad = True
                Else
                    rowScadPagCa.DataFC = ""
                    txtDFC.BackColor = SEGNALA_OK
                End If
            Else
                rowScadPagCa.DataFC = ""
                txtDFC.BackColor = SEGNALA_KO
            End If
            '-
            idxRow += 1
        Next
        '-
        Dim myScadPagCA As String = ""
        Dim TotRate As Decimal = 0
        Dim TotFatturato As Decimal = 0
        Dim TotResiduo As Decimal = 0
        For i = 0 To ArrScadPagCA.Count - 1
            If myScadPagCA.Trim <> "" Then myScadPagCA += ";"
            rowScadPagCa = ArrScadPagCA(i)
            myScadPagCA += rowScadPagCa.NRata.Trim & ";"
            myScadPagCA += rowScadPagCa.Data.Trim & ";"
            myScadPagCA += rowScadPagCa.Importo.Trim & ";"
            myScadPagCA += rowScadPagCa.Evasa.ToString.Trim & ";"
            myScadPagCA += rowScadPagCa.NFC.Trim & ";"
            myScadPagCA += rowScadPagCa.DataFC.Trim & ";"
            myScadPagCA += rowScadPagCa.Serie.Trim & ";"
            'giu191223
            If CBool(rowScadPagCa.Evasa) Then
                If rowScadPagCa.ImportoF.Trim = "" Or rowScadPagCa.ImportoF.Trim = "0" Then
                    rowScadPagCa.ImportoF = rowScadPagCa.Importo
                    rowScadPagCa.ImportoR = "0"
                End If
            Else
                If rowScadPagCa.ImportoR.Trim = "" Or rowScadPagCa.ImportoR.Trim = "0" Then
                    rowScadPagCa.ImportoF = "0"
                    rowScadPagCa.ImportoR = rowScadPagCa.Importo
                ElseIf rowScadPagCa.NFC.Trim = "" And rowScadPagCa.DataFC.Trim = "" Then
                    rowScadPagCa.ImportoF = "0"
                    rowScadPagCa.ImportoR = rowScadPagCa.Importo
                End If
            End If
            myScadPagCA += rowScadPagCa.ImportoF.Trim & ";"
            myScadPagCA += rowScadPagCa.ImportoR.Trim
            '
            Try
                TotRate += CDec(rowScadPagCa.Importo)
                TotFatturato += CDec(rowScadPagCa.ImportoF)
                TotResiduo += CDec(rowScadPagCa.ImportoR)
            Catch ex As Exception

            End Try
        Next
        GridViewDettCASC.DataSource = ArrScadPagCA
        GridViewDettCASC.DataBind()
        '''lblNumRate.Text = "Totale rate: " & FormattaNumero(NRate)
        LblTotaleRate.Text = Format(TotRate, FormatoValEuro)
        lblTotFatturato.Text = Format(TotFatturato, FormatoValEuro)
        lblTotResiduo.Text = Format(TotResiduo, FormatoValEuro)
        'OK AGGIORNO
        Try
            'giu270220
            Dim strErrore As String = ""
            If UPGScadPagCA(myID.Trim, ArrScadPagCA, myScadPagCA, strErrore) = False Then
                ArrScadPagCA = Session(CSTSCADPAGCA)
                GridViewDettCASC.DataSource = ArrScadPagCA
                GridViewDettCASC.DataBind()
                '-
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore Aggiorna Scadenze", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Else
                Session(CSTSCADPAGCA) = ArrScadPagCA
            End If
            If Me.UpgStatoDoc(False, SWOKPagScad, strErrore) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore Aggiorna Stato documento", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            ArrScadPagCA = Session(CSTSCADPAGCA)
            GridViewDettCASC.DataSource = ArrScadPagCA
            GridViewDettCASC.DataBind()
            '-
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Aggiorna Scadenze", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FINE
        Call SaveScadenze() 'giu151223
        GridViewDettCASC.Enabled = False
        btnModScPagCA.Visible = True
        btnAggScPagCA.Visible = False
        btnAnnScPagCA.Visible = False
        btnAggFCFromOC.Enabled = True
        btnAggDataScAtt.Enabled = True
        'controllo attivita non evase SEGNALAZIONE
        Dim strSegnala As String = ""
        SWOKPagScad = False : Dim SWOKAttFatt As Boolean = False
        Call CKAttEvPagEv(strSegnala, , SWOKPagScad, SWOKAttFatt)
        '-
        If SWOKPagScad = False And SWOKAttFatt = True Then
            If strSegnala.Trim <> "" Then
                strSegnala += "<br>"
            End If
            strSegnala += "!!ATTENZIONE!!Sono presenti attività Fatturate ma nessuna Scadenza risulta Fatturata!!<br>" + _
            "Si vuole impostare a NON FATTURATO per tutte le Attività ?"
            Session(MODALPOPUP_CALLBACK_METHOD) = "AggScadAttNOFatt"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strSegnala, WUC_ModalPopup.TYPE_CONFIRM_YN)
            Exit Sub
        End If
        If strSegnala.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strSegnala, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
    End Sub
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

    Private Function SetCdmDAdp() As Boolean

        SqlConnDocDett = New SqlConnection
        SqlAdapDocDett = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDocDett.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
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
        '---------------------------
        SqlDbSelectCmd.CommandText = "get_ConDByIDDocumenti" 'ok select *
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = Me.SqlConnDocDett
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        SqlDbSelectCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        SqlAdapDocDett.SelectCommand = SqlDbSelectCmd

    End Function

    Private Function UpgStatoDoc(ByRef SWOKEvasa As Boolean, ByRef SWOKPagScad As Boolean, ByRef strErrore As String) As Boolean
        UpgStatoDoc = False
        strErrore = ""
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            strErrore = "Errore: SESSIONE SCADENZE SCADUTA (ID) - Riprovate la modifica uscendo e rientrando"
            Exit Function
        End If
        '-
        Dim AllEvasa As Boolean = True
        Dim chkEvasa As CheckBox
        If SWOKEvasa = False Then
            For Each row As GridViewRow In GridViewDettCAAtt.Rows
                chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
                ' ''txtNFC = CType(row.FindControl("txtNFC"), TextBox)
                ' ''txtDFC = CType(row.FindControl("txtDataFC"), TextBox)
                If chkEvasa.Checked Then
                    SWOKEvasa = True
                Else
                    AllEvasa = False
                End If
            Next
        End If
        '-
        Dim AllPagScad As Boolean = True
        If SWOKPagScad = False Then
            For Each row As GridViewRow In GridViewDettCASC.Rows
                chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
                ' ''txtNFC = CType(row.FindControl("txtNFC"), TextBox)
                ' ''txtDFC = CType(row.FindControl("txtDataFC"), TextBox)
                If chkEvasa.Checked = True Then
                    SWOKPagScad = True
                Else
                    AllPagScad = False
                End If
            Next
        End If
        'ok
        Dim NewStatoDoc As Integer = 0
        If SWOKEvasa = False And SWOKPagScad = False Then
            NewStatoDoc = 0
        ElseIf SWOKEvasa = True Or SWOKPagScad = True Then
            If AllEvasa = True And AllPagScad = True Then
                NewStatoDoc = 1
            Else
                NewStatoDoc = 2
            End If
        End If
        Dim strSQL As String = ""
        Dim SWOk As Boolean = True
        strSQL = "UPDATE ContrattiT SET StatoDoc=" & NewStatoDoc.ToString.Trim & " Where IDDocumenti=" & myID.Trim
        '-
        Dim ObjDB As New DataBaseUtility
        Try
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL)
            If SWOk = False Then
                ObjDB = Nothing
                strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata StatoDoc"
                Exit Function
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata StatoDoc - " & Ex.Message.Trim
            Exit Function
        End Try
        UpgStatoDoc = True
    End Function

    Public Function CKAttEvPagEv(Optional ByRef strSegnala As String = "", Optional ByRef SWOKAttEvasa As Boolean = False, _
                                 Optional ByRef SWOKPagScad As Boolean = False, _
                                 Optional ByRef SWOKAttFatt As Boolean = False) As Boolean
        strSegnala = ""
        CKAttEvPagEv = False
        'giu271123 controllo se tutte le fatture siano state emesse cosi posso richiedere se togliere flag fatturato sulle singole attività fatturate
        SWOKPagScad = False
        SWOKAttEvasa = False
        SWOKAttFatt = False
        '--
        Dim chkEvasa As CheckBox
        Dim chkFatturata As CheckBox
        Dim FinoAAAA As Integer = -1
        Dim txtDFC As TextBox
        Dim txtNumFC As TextBox
        Dim txtDScAtt As TextBox
        'GIU240821 IL CONTROLLO DEV'ESSERE FATTO SULLA SCADENZA ATTIVITA' (FATTURA) E NON LA DATA DI FATTURAZIONE CHE PUO' ESSERE =, SUCCESSIVA O PRECEDENTE
        Try
            '-Scadenzario
            For Each row As GridViewRow In GridViewDettCASC.Rows
                chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
                txtDScAtt = CType(row.FindControl("txtDataScAtt"), TextBox)
                txtDFC = CType(row.FindControl("txtDataFC"), TextBox)
                txtNumFC = CType(row.FindControl("txtNFC"), TextBox)
                If chkEvasa.Checked = True Then
                    SWOKPagScad = True
                ElseIf txtDFC.Text.Trim <> "" Or txtNumFC.Text.Trim <> "" Then
                    SWOKPagScad = True
                End If
                If IsDate(txtDFC.Text) Then
                    If CDate(txtDFC.Text).Year > FinoAAAA Then
                        FinoAAAA = CDate(txtDScAtt.Text).Year
                    End If
                ElseIf IsNumeric(txtDFC.Text.Trim) Then 'nel caso ci sia solo l'anno 
                    If Int(txtDFC.Text.Trim) > FinoAAAA Then
                        FinoAAAA = Int(txtDFC.Text.Trim)
                    End If
                End If
            Next
            '-Attivita
            'GIU201223
            If Not (Session(CSTSCADATTCA) Is Nothing) Then
                DVScadAtt = Session(CSTSCADATTCA)
                DVScadAtt.RowFilter = "Qta_Evasa<>0"
                SWOKAttEvasa = CBool(DVScadAtt.Count)
                DVScadAtt.RowFilter = "Qta_Fatturata<>0"
                SWOKAttFatt = CBool(DVScadAtt.Count)
                DVScadAtt.RowFilter = ""
            Else
                lblTotaleAtt.Text = ""
            End If
            '---------
            '''    For Each row As GridViewRow In GridViewDettCAAtt.Rows
            '''        chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
            '''        chkFatturata = CType(row.FindControl("chkFatturata"), CheckBox)
            '''        txtDFC = CType(row.FindControl("txtDataSC"), TextBox)
            '''        If chkEvasa.Checked = True Then
            '''            SWOKAttEvasa = True
            '''        Else
            '''            If Not strSegnala.Trim.Contains("non ancora evase") Then 'giu280520 segnalo che vi sono delle attività non chiuse a fronte di FATTURE EMESSE 
            '''                If IsDate(txtDFC.Text) Then
            '''                    If CDate(txtDFC.Text).Year <= FinoAAAA Then
            '''                        If strSegnala.Trim <> "" Then
            '''                            strSegnala += "<br>"
            '''                        End If
            '''                        strSegnala += "Attenzione, sono presenti attività non ancora evase a fronte di fattura emessa. Periodo fino al: " & FinoAAAA.ToString.Trim
            '''                    End If
            '''                ElseIf IsNumeric(txtDFC.Text) Then 'nel caso ci sia solo l'anno 
            '''                    If CDate(txtDFC.Text).Year <= FinoAAAA Then
            '''                        If strSegnala.Trim <> "" Then
            '''                            strSegnala += "<br>"
            '''                        End If
            '''                        strSegnala += "Attenzione, sono presenti attività non ancora evase a fronte di fattura emessa. Periodo fino al: " & FinoAAAA.ToString.Trim
            '''                    End If
            '''                End If
            '''            End If
            '''        End If
            '''        If chkFatturata.Checked Then
            '''            SWOKAttFatt = True
            '''        Else
            '''            If Not strSegnala.Trim.Contains("non ancora Fatturate") Then  'giu271123 segnalo che vi sono delle attività non fatturate a fronte di FATTURE EMESSE 
            '''                If IsDate(txtDFC.Text) Then
            '''                    If CDate(txtDFC.Text).Year <= FinoAAAA Then
            '''                        If strSegnala.Trim <> "" Then
            '''                            strSegnala += "<br>"
            '''                        End If
            '''                        strSegnala = "Attenzione, sono presenti attività non ancora Fatturate a fronte di fattura emessa. Periodo fino al: " & FinoAAAA.ToString.Trim
            '''                    End If
            '''                ElseIf IsNumeric(txtDFC.Text) Then 'nel caso ci sia solo l'anno 
            '''                    If CDate(txtDFC.Text).Year <= FinoAAAA Then
            '''                        If strSegnala.Trim <> "" Then
            '''                            strSegnala += "<br>"
            '''                        End If
            '''                        strSegnala = "Attenzione, sono presenti attività non ancora Fatturate a fronte di fattura emessa. Periodo fino al: " & FinoAAAA.ToString.Trim
            '''                    End If
            '''                End If
            '''            End If
            '''        End If
            '''    Next
        Catch ex As Exception

        End Try

        If SWOKPagScad = True Or SWOKAttEvasa = True Or SWOKAttFatt = True Then 'giu291123 anche se fatturate non posso rigenerare
            CKAttEvPagEv = True
        End If
    End Function
    'giu281123 se non è presente alcuna RATA FATTURATA assegno a NO FATTURATO alle attività
    Public Function AggScadAttNOFatt() As Boolean
        Session(CALLGESTIONE) = SWNO
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        If GridViewDettCAAtt.Rows.Count > 0 Then
            'OK
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato presente.", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session(CSTSCADATTCA)
            DVScadAttSAVE = New DataView(DVScadAtt.ToTable)
            Session("DVScadAttSAVE") = DVScadAttSAVE
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'AGGORNO SUL DB
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
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataEV", System.Data.SqlDbType.DateTime, 8, "DataEV"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 8, "RefDataNC"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Fatturata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Fatturata", System.Data.DataRowVersion.Current, Nothing))
        If SqlConn.State <> ConnectionState.Open Then
            SqlConn.Open()
        End If
        '---------------------------
        'ok ora procedo
        DVScadAtt.RowFilter = ""
        strErrore = ""
        Dim RowsAggiorna() As DataRow = DVScadAtt.Table.Select("Qta_Fatturata<>0")
        For Each rowAggiorna As DataRow In RowsAggiorna
            rowAggiorna.Item("Qta_Fatturata") = 0
            Try
                SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = rowAggiorna.Item("IDDocumenti")
                SqlUpd_ConTScadPag.Parameters("@DurataNum").Value = rowAggiorna.Item("DurataNum")
                SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = rowAggiorna.Item("DurataNumRiga")
                SqlUpd_ConTScadPag.Parameters("@Riga").Value = rowAggiorna.Item("Riga")
                SqlUpd_ConTScadPag.Parameters("@Qta_Evasa").Value = rowAggiorna.Item("Qta_Evasa")
                SqlUpd_ConTScadPag.Parameters("@Qta_Residua").Value = rowAggiorna.Item("Qta_Residua")
                SqlUpd_ConTScadPag.Parameters("@DataSC").Value = CDate(rowAggiorna.Item("TextDataSc"))
                If IsDBNull(rowAggiorna.Item("DataEv")) Then
                    SqlUpd_ConTScadPag.Parameters("@DataEV").Value = DBNull.Value
                Else
                    SqlUpd_ConTScadPag.Parameters("@DataEV").Value = CDate(rowAggiorna.Item("DataEv"))
                End If
                'giu080223
                If IsDBNull(rowAggiorna.Item("TextRefDataNC")) Then
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                ElseIf rowAggiorna.Item("TextRefDataNC").ToString.Trim = "" Or Not IsDate(rowAggiorna.Item("TextRefDataNC").ToString.Trim) Then
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                Else
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = CDate(rowAggiorna.Item("TextRefDataNC").ToString.Trim)
                End If
                SqlUpd_ConTScadPag.Parameters("@Qta_Fatturata").Value = rowAggiorna.Item("Qta_Fatturata")
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
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            Finally
                'nulla
            End Try
        Next
        If Not IsNothing(SqlConn) Then
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
                SqlConn = Nothing
            End If
        End If
        '-------------------------------
        Session(CSTSCADATTCA) = DVScadAtt
        lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
        DVScadAtt.RowFilter = "Qta_Evasa<>0"
        lblTotaleAtt.Text += " di cui Evase: " & DVScadAtt.Count.ToString.Trim
        '-
        DVScadAtt.RowFilter = "Qta_Fatturata<>0"
        lblTotaleAtt.Text += " Fatturate: " & DVScadAtt.Count.ToString.Trim
        '-
        DVScadAtt.RowFilter = ""
        If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,Serie"
        Session(CSTSCADATTCA) = DVScadAtt
        GridViewDettCAAtt.DataSource = DVScadAtt
        GridViewDettCAAtt.DataBind()
        '-------------------------------
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Aggiornamento Attività: NON FATTURATA.", "Aggiornamento terminato con successo.", WUC_ModalPopup.TYPE_INFO)
        Exit Function
    End Function
#End Region

    Private Sub txtAbbuono_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtAbbuono.TextChanged
        Call TotaleNettoPag()
        Session(SWMODIFICATO) = SWSI
        Dim strErrore As String = ""
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        Dim DecimaliValutaFinito As Integer = Int(DecimaliVal)
        '------------------------------------
        myTipoDoc = Session(CSTTIPODOC)
        If IsNothing(myTipoDoc) Then
            myTipoDoc = ""
        End If
        If String.IsNullOrEmpty(myTipoDoc) Then
            myTipoDoc = ""
        End If
        If myTipoDoc = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore: TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '---------------------------------------------------------------
        If Calcola_ScadenzeCA(myTipoDoc, DecimaliValutaFinito, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Sub
        End If
    End Sub

    Private Sub chkNoDivTotRate_Chk_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        chkNoDivTotRate.CheckedChanged, chkAccorpaRateAA.CheckedChanged
        Session(CALLGESTIONE) = SWNO
        Dim strErrore As String = ""
        Session(SWMODIFICATO) = SWSI
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        Dim DecimaliValutaFinito As Integer = Int(DecimaliVal)
        '------------------------------------
        myTipoDoc = Session(CSTTIPODOC)
        If IsNothing(myTipoDoc) Then
            myTipoDoc = ""
        End If
        If String.IsNullOrEmpty(myTipoDoc) Then
            myTipoDoc = ""
        End If
        If myTipoDoc = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore: TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '---------------------------------------------------------------
        If Calcola_ScadenzeCA(myTipoDoc, DecimaliValutaFinito, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Sub
        End If
        
    End Sub

    Private Sub btnAggFCFromOC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggFCFromOC.Click
        Session(CALLGESTIONE) = SWNO
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If GridViewDettCASC.Rows.Count > 0 Then
            'OK
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna Rata di scadenza è presente.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        ArrScadPagCA = New ArrayList
        If Not (Session(CSTSCADPAGCA) Is Nothing) Then
            ArrScadPagCA = Session(CSTSCADPAGCA)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(CALLGESTIONE) = SWNO
        Dim strNonAbbinati As String = ""
        If GetAndAbbinaFCToCM(strNonAbbinati) = True Then
            If strNonAbbinati.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Aggiorna N° e Data Fattura", "Si prega di verificare e confermare l'aggiornamento <br> Alcuni documenti non sono stati abbinati: <br>" + strNonAbbinati.Trim, WUC_ModalPopup.TYPE_ALERT)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Aggiorna N° e Data Fattura", "Operazione di abbinamento completata. <br> Si prega di verificare e confermare l'aggiornamento <br>" + strNonAbbinati.Trim, WUC_ModalPopup.TYPE_INFO)
            End If
            subSaveScadPagCA()
        End If

    End Sub
    Private Sub subSaveScadPagCA()
        If (Session("DsDocT") Is Nothing) Then
            Exit Sub
        End If
        Dim DsDocT As New DSDocumenti
        DsDocT = Session("DsDocT")
        Dim dvDocT As DataView
        dvDocT = New DataView(DsDocT.ContrattiT)
        Dim NRate As Integer = 0
        Try
            If IsDBNull(dvDocT.Item(0).Item("ScadPagCA")) Then
                dvDocT.Item(0).Item("ScadPagCA") = ""
            End If
            ArrScadPagCA = New ArrayList
            Dim myScadCA As ScadPagCAEntity
            If dvDocT.Item(0).Item("ScadPagCA") <> "" Then
                Dim lineaSplit As String() = dvDocT.Item(0).Item("ScadPagCA").Split(";")
                For i = 0 To lineaSplit.Count - 1
                    If lineaSplit(i).Trim <> "" And (i + 8) <= lineaSplit.Count - 1 Then 'GIU191223 DA i + 6

                        myScadCA = New ScadPagCAEntity
                        myScadCA.NRata = lineaSplit(i).Trim
                        i += 1
                        myScadCA.Data = lineaSplit(i).Trim
                        i += 1
                        myScadCA.Importo = lineaSplit(i).Trim
                        i += 1
                        myScadCA.Evasa = lineaSplit(i).Trim
                        i += 1
                        myScadCA.NFC = lineaSplit(i).Trim
                        i += 1
                        myScadCA.DataFC = lineaSplit(i).Trim
                        i += 1
                        myScadCA.Serie = lineaSplit(i).Trim
                        i += 1
                        myScadCA.ImportoF = lineaSplit(i).Trim
                        i += 1
                        myScadCA.ImportoR = lineaSplit(i).Trim
                        ArrScadPagCA.Add(myScadCA)
                        NRate += 1
                    End If
                Next
            End If
            Session(CSTSCADPAGCA) = ArrScadPagCA
            '''GridViewDettCASC.DataSource = ArrScadPagCA
            '''GridViewDettCASC.DataBind()
        Catch ex As Exception

        End Try
    End Sub

    'giu271023 riporto fatture nel Scadenzario Rate da fatturare
    Private Function GetAndAbbinaFCToCM(ByRef strNonAbbinati As String) As Boolean
        GetAndAbbinaFCToCM = False
        strNonAbbinati = ""
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
            ModalPopup.Show("Errore: SESSIONE SCADENZE SCADUTA (ID)", "Riprovate la modifica uscendo e rientrando (GetAndAbbinaFCToCM).", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        '-
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        '''Dim SqlConnDoc As New SqlConnection
        '''SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-
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
                    rowPag = dsT.Tables(0).Select("Tipo_Doc='OC'")
                    If rowPag.Length = 0 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Aggiorna N° e Data Fattura da Ordine collegato", "Nessun Ordine collegato.", WUC_ModalPopup.TYPE_INFO)
                        Exit Function
                    End If
                    '-
                    rowPag = dsT.Tables(0).Select("Tipo_Doc='FC'")
                    If rowPag.Length = 0 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Aggiorna N° e Data Fattura da Ordine collegato", "Nessuna Fattura emessa per l'Ordine collegato.", WUC_ModalPopup.TYPE_INFO)
                        Exit Function
                    End If
                    ArrScadPagCA = New ArrayList
                    If Not (Session(CSTSCADPAGCA) Is Nothing) Then
                        ArrScadPagCA = Session(CSTSCADPAGCA)
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
                        Exit Function
                    End If
                    Dim SWUnicaRata As Boolean = False
                    If ArrScadPagCA.Count = 1 Then
                        SWUnicaRata = True
                    End If
                    Dim myAnnoFC As String = ""
                    Dim myDataFC As String = "" : Dim myNumFC As String = "" : Dim myFCPA As String
                    Dim myTotNettoPagare As String = ""
                    Dim SWOKAbbinati As Boolean = False
                    Dim myScadCA As ScadPagCAEntity
                    For Each row In dsT.Tables(0).Select("Tipo_Doc='FC'")
                        myNumFC = IIf(IsDBNull(row.Item("Numero")), "", row.Item("Numero"))
                        myDataFC = IIf(IsDBNull(row.Item("Data_Doc")), "", row.Item("Data_Doc"))
                        myFCPA = IIf(IsDBNull(row.Item("FatturaPA")), "", row.Item("FatturaPA"))
                        myTotNettoPagare = IIf(IsDBNull(row.Item("TotNettoPagare")), "0", row.Item("TotNettoPagare").ToString.Trim)
                        myAnnoFC = ""
                        If IsDate(myDataFC) Then
                            myAnnoFC = CDate(myDataFC).Year.ToString.Trim
                        End If
                        If myAnnoFC.Trim <> "" And myNumFC.Trim <> "" Then
                            myScadCA = New ScadPagCAEntity
                            If SWUnicaRata Then
                                myScadCA = ArrScadPagCA(0) 'Where x.Data.ToString.Trim.ToUpper.Contains(myAnnoFC.Trim.ToUpper)
                            Else
                                Dim Trovato = From x In ArrScadPagCA Where x.Data.ToString.Trim.ToUpper.Contains(myAnnoFC.Trim.ToUpper)
                                myScadCA = Trovato(0)
                            End If
                            '-
                            If Not (myScadCA Is Nothing) Then
                                Try
                                    If myScadCA.DataFC.ToString.Trim = "" Then
                                        myScadCA.DataFC = myDataFC
                                    End If
                                    If myScadCA.NFC.ToString.Trim = "" Then
                                        myScadCA.NFC = myNumFC + myFCPA.Trim
                                    ElseIf myScadCA.NFC.ToString.Contains(myNumFC) Then
                                        'nulla
                                    Else
                                        myScadCA.NFC += "," + myNumFC + myFCPA.Trim
                                    End If
                                    'GIU251123
                                    Try
                                        If String.IsNullOrEmpty(myTotNettoPagare) Then
                                            myTotNettoPagare = 0
                                            myScadCA.Evasa = False
                                            myScadCA.ImportoF = "0"
                                        ElseIf String.IsNullOrEmpty(myScadCA.Importo) Then
                                            myTotNettoPagare = 0
                                            myScadCA.Evasa = False
                                            myScadCA.ImportoF = "0"
                                        ElseIf CDec(myTotNettoPagare.Trim) = CDec(myScadCA.Importo) Then
                                            myScadCA.Evasa = True
                                            myScadCA.ImportoF = myTotNettoPagare.Trim
                                            myScadCA.ImportoR = "0"
                                        ElseIf CDec(myTotNettoPagare.Trim) > CDec(myScadCA.Importo) Then
                                            myScadCA.Evasa = True
                                            myScadCA.ImportoF = myScadCA.Importo
                                            myScadCA.ImportoR = "0"
                                        ElseIf CDec(myTotNettoPagare.Trim) < CDec(myScadCA.Importo) Then
                                            '''myScadCA.Evasa = False
                                            '''myScadCA.ImportoF = myScadCA.Importo
                                            '''myScadCA.ImportoR = "0"
                                            myScadCA.ImportoF = (CDec(myScadCA.ImportoF) + CDec(myTotNettoPagare)).ToString.Trim
                                            If CDec(myScadCA.ImportoF) > CDec(myScadCA.Importo.Trim) Then
                                                myScadCA.Evasa = True
                                                myScadCA.ImportoF = myScadCA.Importo.Trim
                                                myScadCA.ImportoR = "0"
                                            Else
                                                myScadCA.Evasa = False
                                                myScadCA.ImportoR = (CDec(myScadCA.ImportoF) - CDec(myScadCA.Importo.Trim)).ToString.Trim
                                            End If
                                        Else
                                            myScadCA.Evasa = False
                                            'myScadCA.ImportoF = "0"
                                        End If
                                    Catch ex As Exception
                                        myScadCA.Evasa = False
                                    End Try
                                    SWOKAbbinati = True
                                Catch ex As Exception
                                    strNonAbbinati += " - FC N° " + myNumFC + myFCPA.Trim + " del " + myDataFC + myFCPA.Trim
                                End Try
                            Else
                                strNonAbbinati += " - FC N° " + myNumFC + myFCPA.Trim + " del " + myDataFC + myFCPA.Trim
                            End If
                        End If
                    Next
                    If SWOKAbbinati Then
                        GridViewDettCASC.DataSource = ArrScadPagCA
                        GridViewDettCASC.DataBind()
                        '-
                        Session(SWOP) = SWOPMODSCATT
                        Session(SWOPMODSCATT) = SWSI
                        GridViewDettCASC.Enabled = True
                        btnModScPagCA.Visible = False
                        btnAggScPagCA.Visible = True
                        btnAnnScPagCA.Visible = True
                    End If
                    GetAndAbbinaFCToCM = True
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Aggiorna N° e Data Fattura da Ordine collegato", "Nessun Ordine collegato.", WUC_ModalPopup.TYPE_INFO)
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Aggiorna N° e Data Fattura da Ordine collegato", "Nessun Ordine collegato.", WUC_ModalPopup.TYPE_INFO)
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
    'giu041123
    Private Sub btnAggDataScAtt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggDataScAtt.Click
        Session(CALLGESTIONE) = SWNO
        If Session(SWOPMODSCATT) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Scadenze/Attivita", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Session(SWOPDETTDOCL) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione <br> Modifica Note intervento", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'Controllo Dati inseriti
        If chkTutteLeDate.Checked = False Then
            If Not IsDate(txtDataOLD.Text.Trim) Or Not IsDate(txtDataNEW.Text.Trim) Or _
               txtDataOLD.Text.Trim.Length <> 10 Or txtDataNEW.Text.Trim.Length <> 10 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "DATI ERRATI: Data da cambiare in Nuova data ", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            ElseIf CDate(txtDataOLD.Text).Year <> CDate(txtDataNEW.Text).Year Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", " ANNO di entrambe le date dev'essere uguale", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            ElseIf CDate(txtDataOLD.Text).Date = CDate(txtDataNEW.Text).Date Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", " Le date devono essere diverse", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        Else
            If Not IsDate(txtDataNEW.Text.Trim) Or txtDataNEW.Text.Trim.Length <> 10 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "DATI ERRATI: Nuova data errata", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
                'qui comefare per tutte le date 30/11/2023 --- 2024 ---solo il mese anno sempre lo stesso
            End If
        End If
        '-----------------------
        Session(CALLGESTIONE) = SWNO
        If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session(CSTSCADATTCA)
            If DVScadAtt.Count > 0 Then
                'OK saldo lasituazione attuale
                DVScadAttSAVE = New DataView(DVScadAtt.ToTable)
                Session("DVScadAttSAVE") = DVScadAttSAVE
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessuna Attività è presente.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu041123
        Dim NSL As String = DDLDurNumRiga.SelectedItem.Text
        NSL = Trim(Mid(NSL, InStr(NSL, "-") + 1))
        If NSL.ToUpper = "[Tutti]".ToUpper Then NSL = ""
        '-
        Dim pCodVisita As String = "" : Dim pNVisite As Integer = 0
        If GetCodVisitaDATipoCAIdPag(pCodVisita, pNVisite) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando. Definizione Tipo Contratto errato.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '---------
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
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataEV", System.Data.SqlDbType.DateTime, 8, "DataEV"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 8, "RefDataNC"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Fatturata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Fatturata", System.Data.DataRowVersion.Current, Nothing))
        If SqlConn.State <> ConnectionState.Open Then
            SqlConn.Open()
        End If
        '---------------------------
        'modifica date scadenze sempre valide per tutte
        '---------------------------
        strErrore = ""
        DVScadAtt.RowFilter = ""
        Dim RowsAggiorna() As DataRow = DVScadAtt.Table.Select("", "DurataNumRiga,SerieLotto")
        Dim swOKDateOLD As Boolean = False : Dim SWAgg As Boolean = False
        Dim strDataCKPrec As String = "" : Dim strDataConsPrec As String = "" : Dim strDNRPrec As String = "" : Dim strSeriePrec As String = ""
        Dim strSegnalaDateDiverse As String = ""
        For Each rowAggiorna As DataRow In RowsAggiorna
            Try
                '-
                SWAgg = False
                If rowAggiorna.Item("Qta_Evasa") = 0 Then
                    If chkTutteLeDate.Checked = False Then
                        If NSL.Trim = "" Then
                            If CDate(rowAggiorna.Item("TextDataSc")).Date = CDate(txtDataOLD.Text).Date Then
                                If chkNonAggConsum.Checked = False Then
                                    SWAgg = True
                                ElseIf IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                                    SWAgg = True
                                ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                                    SWAgg = True
                                End If
                                If SWAgg = True Then
                                    rowAggiorna.Item("TextDataSc") = txtDataNEW.Text
                                End If
                            End If
                        Else
                            If CDate(rowAggiorna.Item("TextDataSc")).Date = CDate(txtDataOLD.Text).Date And _
                                NSL.Trim.ToUpper = rowAggiorna.Item("SerieLotto").ToString.Trim.ToUpper Then
                                If chkNonAggConsum.Checked = False Then
                                    SWAgg = True
                                ElseIf IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                                    SWAgg = True
                                ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                                    SWAgg = True
                                End If
                                If SWAgg = True Then
                                    rowAggiorna.Item("TextDataSc") = txtDataNEW.Text
                                End If
                            End If
                        End If
                    Else
                        If NSL.Trim = "" Then
                            If chkNonAggConsum.Checked = False Then
                                SWAgg = True
                            ElseIf IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                                SWAgg = True
                            ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                                SWAgg = True
                            End If
                            '-
                            If DDLPeriodo.SelectedIndex > 0 Then
                                If DDLPeriodo.SelectedValue.Trim <> rowAggiorna.Item("DurataNumRiga").ToString.Trim Then
                                    SWAgg = False
                                End If
                            End If
                            If SWAgg = True Then
                                rowAggiorna.Item("TextDataSc") = Mid(txtDataNEW.Text, 1, 6) + CDate(rowAggiorna.Item("TextDataSc")).Year.ToString.Trim
                            End If
                        Else
                            If NSL.Trim.ToUpper = rowAggiorna.Item("SerieLotto").ToString.Trim.ToUpper Then
                                If chkNonAggConsum.Checked = False Then
                                    SWAgg = True
                                ElseIf IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                                    SWAgg = True
                                ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                                    SWAgg = True
                                End If
                                '-
                                If DDLPeriodo.SelectedIndex > 0 Then
                                    If DDLPeriodo.SelectedValue.Trim <> rowAggiorna.Item("DurataNumRiga").ToString.Trim Then
                                        SWAgg = False
                                    End If
                                End If
                                If SWAgg = True Then
                                    rowAggiorna.Item("TextDataSc") = Mid(txtDataNEW.Text, 1, 6) + CDate(rowAggiorna.Item("TextDataSc")).Year.ToString.Trim
                                End If
                            End If
                        End If
                    End If
                    'okcontrolli
                    If strDNRPrec = "" Then
                        strDNRPrec = rowAggiorna.Item("DurataNumRiga")
                        strSeriePrec = rowAggiorna.Item("SerieLotto")
                        strDataCKPrec = "" : strDataConsPrec = ""
                        If IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                            '-
                        ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                            strDataCKPrec = rowAggiorna.Item("TextDataSc")
                        Else
                            strDataConsPrec = rowAggiorna.Item("TextDataSc")
                        End If
                    End If
                    If strDNRPrec <> rowAggiorna.Item("DurataNumRiga") Or strSeriePrec <> rowAggiorna.Item("SerieLotto") Then
                        If IsDate(strDataCKPrec) And IsDate(strDataConsPrec) Then
                            If CDate(strDataCKPrec).Date > CDate(strDataConsPrec).Date Then
                                strSegnalaDateDiverse += "(" + strSeriePrec + ") Data Ver DAE superiore alla data del Consumabile " + strDataCKPrec + "-" + strDataConsPrec
                            ElseIf CDate(strDataCKPrec).Date <> CDate(strDataConsPrec).Date Then
                                strSegnalaDateDiverse += "(" + strSeriePrec + ") Data Ver DAE diversa alla data del Consumabile " + strDataCKPrec + "-" + strDataConsPrec
                            End If
                        End If
                        '-
                        strDNRPrec = rowAggiorna.Item("DurataNumRiga")
                        strSeriePrec = rowAggiorna.Item("SerieLotto")
                        strDataCKPrec = "" : strDataConsPrec = ""
                        If IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                            '-
                        ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                            strDataCKPrec = rowAggiorna.Item("TextDataSc")
                        Else
                            strDataConsPrec = rowAggiorna.Item("TextDataSc")
                        End If
                    Else
                        If IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                            '-
                        ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                            If IsDate(strDataCKPrec) And IsDate(rowAggiorna.Item("TextDataSc")) Then
                                If CDate(strDataCKPrec).Date < CDate(rowAggiorna.Item("TextDataSc")).Date Then
                                    strDataCKPrec = rowAggiorna.Item("TextDataSc")
                                End If
                            Else
                                strDataCKPrec = rowAggiorna.Item("TextDataSc")
                            End If
                        Else
                            If IsDate(strDataConsPrec) And IsDate(rowAggiorna.Item("TextDataSc")) Then
                                If CDate(strDataConsPrec).Date < CDate(rowAggiorna.Item("TextDataSc")).Date Then
                                    strDataConsPrec = rowAggiorna.Item("TextDataSc")
                                End If
                            Else
                                strDataConsPrec = rowAggiorna.Item("TextDataSc")
                            End If
                        End If
                    End If
                End If
                If SWAgg = True Then
                    swOKDateOLD = True
                    Try
                        SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = rowAggiorna.Item("IDDocumenti")
                        SqlUpd_ConTScadPag.Parameters("@DurataNum").Value = rowAggiorna.Item("DurataNum")
                        SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = rowAggiorna.Item("DurataNumRiga")
                        SqlUpd_ConTScadPag.Parameters("@Riga").Value = rowAggiorna.Item("Riga")
                        SqlUpd_ConTScadPag.Parameters("@Qta_Evasa").Value = rowAggiorna.Item("Qta_Evasa")
                        SqlUpd_ConTScadPag.Parameters("@Qta_Residua").Value = rowAggiorna.Item("Qta_Residua")
                        SqlUpd_ConTScadPag.Parameters("@DataSC").Value = CDate(rowAggiorna.Item("TextDataSc"))
                        If IsDBNull(rowAggiorna.Item("DataEv")) Then
                            SqlUpd_ConTScadPag.Parameters("@DataEV").Value = DBNull.Value
                        Else
                            SqlUpd_ConTScadPag.Parameters("@DataEV").Value = CDate(rowAggiorna.Item("DataEv"))
                        End If
                        If IsDBNull(rowAggiorna.Item("TextRefDataNC")) Then
                            SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                        ElseIf rowAggiorna.Item("TextRefDataNC").ToString.Trim = "" Or Not IsDate(rowAggiorna.Item("TextRefDataNC").ToString.Trim) Then
                            SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                        Else
                            SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = CDate(rowAggiorna.Item("TextRefDataNC").ToString.Trim)
                        End If
                        SqlUpd_ConTScadPag.Parameters("@Qta_Fatturata").Value = rowAggiorna.Item("Qta_Fatturata")
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
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    Catch ex As Exception
                        If Not IsNothing(SqlConn) Then
                            If SqlConn.State <> ConnectionState.Closed Then
                                SqlConn.Close()
                                SqlConn = Nothing
                            End If
                        End If
                        strErrore = ex.Message
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    Finally
                        'nulla
                    End Try
                End If

            Catch ex As Exception
                strErrore = ex.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("(Cambio data Scadenza Attività Errore in aggiornamento date Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            Finally
                'nulla
            End Try
        Next
        If Not IsNothing(SqlConn) Then
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
                SqlConn = Nothing
            End If
        End If
        'ultimo
        If IsDate(strDataCKPrec) And IsDate(strDataConsPrec) Then
            If CDate(strDataCKPrec).Date > CDate(strDataConsPrec).Date Then
                strSegnalaDateDiverse += "(" + strSeriePrec + ") Data Ver DAE superiore alla data del Consumabile " + strDataCKPrec + "-" + strDataConsPrec
            ElseIf CDate(strDataCKPrec).Date <> CDate(strDataConsPrec).Date Then
                strSegnalaDateDiverse += "(" + strSeriePrec + ") Data Ver DAE diversa alla data del Consumabile " + strDataCKPrec + "-" + strDataConsPrec
            End If
        End If
        '-
        lblTotaleAtt.Text = DVScadAtt.Count.ToString.Trim
        DVScadAtt.RowFilter = "Qta_Evasa<>0"
        lblTotaleAtt.Text += " di cui Evase: " & DVScadAtt.Count.ToString.Trim
        DVScadAtt.RowFilter = "Qta_Fatturata<>0"
        lblTotaleAtt.Text += " Fatturate: " & DVScadAtt.Count.ToString.Trim
        '-
        DVScadAtt.RowFilter = ""
        If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,Serie"
        GridViewDettCAAtt.DataSource = DVScadAtt
        GridViewDettCAAtt.DataBind()
        '-------------------------------
        If swOKDateOLD = True Then
            Session(SWOP) = SWOPMODSCATT
            Session(SWOPMODSCATT) = SWSI
            GridViewDettCAAtt.Enabled = True
            btnModScAttCA.Visible = False
            btnAggScAttCA.Visible = True
            btnAnnScAttCA.Visible = True
            btnAggDataScAtt.Enabled = False
            btnAggFCFromOC.Enabled = False
            '-
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Cambio data Scadenza Attività", "Operazione di cambio Data completata. <br> Si prega di verificare e confermare l'aggiornamento <br>" + Mid(strSegnalaDateDiverse, 1, 300), WUC_ModalPopup.TYPE_INFO)
            Call _WucElement.ReBuildDett()
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Cambio data Scadenza Attività", "Nessuna data è stata trovata per il cambio. <br>" + "Cause possibili: Attività già evase!!!<br>" + Mid(strSegnalaDateDiverse, 1, 300), WUC_ModalPopup.TYPE_INFO)
        End If

    End Sub
    Private Function GetCodVisitaDATipoCAIdPag(ByRef pCodVisita As String, ByRef pNVisite As Integer) As Boolean
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

    Private Sub GridViewDettCAAtt_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewDettCAAtt.PageIndexChanging
        '---------------------------
        If AGGPAGScadAtt() = False Then Exit Sub
        GridViewDettCAAtt.EditIndex = -1
        If (e.NewPageIndex = -1) Then
            GridViewDettCAAtt.PageIndex = 0
        Else
            GridViewDettCAAtt.PageIndex = e.NewPageIndex
        End If
        GridViewDettCAAtt.SelectedIndex = -1
        If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session(CSTSCADATTCA)
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        GridViewDettCAAtt.DataSource = DVScadAtt
        GridViewDettCAAtt.DataBind()
    End Sub
    '-
    Private Function AGGPAGScadAtt() As Boolean
        AGGPAGScadAtt = False
        If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session(CSTSCADATTCA)
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'controllo date scadenze sempre valide per tutte
        Dim SWOKEvasa As Boolean = False
        Dim idxRow As Integer = 0
        Dim chkEvasa As CheckBox
        Dim chkFatturata As CheckBox
        Dim txtDSC As TextBox
        Dim txtDEV As TextBox
        Dim txtDSCNext As TextBox
        Dim SWErrore As Boolean = False
        Dim strErrore As String = ""
        Dim Comodo As String = ""
        idxRow = 0
        Dim idxRowDV As Integer = 0
        Dim SWAgg As Boolean = False
        Try
            For Each row As GridViewRow In GridViewDettCAAtt.Rows
                SWErrore = False
                chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
                chkFatturata = CType(row.FindControl("chkFatturata"), CheckBox)
                txtDSC = CType(row.FindControl("txtDataSC"), TextBox)
                txtDEV = CType(row.FindControl("txtDataEv"), TextBox)
                '-
                If txtDSC.Text.Trim.Length <> 10 Then
                    txtDSC.BackColor = SEGNALA_KO
                    SWErrore = True
                ElseIf Not IsDate(txtDSC.Text.Trim) Then
                    txtDSC.BackColor = SEGNALA_KO
                    SWErrore = True
                Else
                    txtDSC.BackColor = SEGNALA_OK
                End If
                If txtDEV.Text.Trim <> "" Then
                    If Not IsDate(txtDEV.Text.Trim) Then
                        txtDEV.BackColor = SEGNALA_KO
                        SWErrore = True
                    Else
                        txtDEV.BackColor = SEGNALA_OK
                    End If
                Else
                    txtDEV.BackColor = SEGNALA_OK
                End If
                'giu080223
                txtDSCNext = CType(row.FindControl("txtDataEVN"), TextBox)
                If txtDSCNext.Text.Trim <> "" Then
                    If Not IsDate(txtDSCNext.Text.Trim) Then
                        txtDSCNext.BackColor = SEGNALA_KO
                        SWErrore = True
                    Else
                        txtDSCNext.BackColor = SEGNALA_OK
                    End If
                Else
                    txtDSCNext.BackColor = SEGNALA_OK
                End If
                If Not SWErrore Then
                    idxRowDV = idxRow + (GridViewDettCAAtt.PageSize * GridViewDettCAAtt.PageIndex)
                    If chkEvasa.Checked Then
                        If DVScadAtt(idxRowDV).Item("Qta_Evasa") = 0 Or txtDEV.Text.Trim = "" Then
                            SWAgg = True
                        End If
                        If IsDBNull(DVScadAtt.Item(idxRowDV).Item("DataEv")) Then
                            SWAgg = True
                        ElseIf DVScadAtt.Item(idxRowDV).Item("DataEv").ToString.Trim = "" Then
                            SWAgg = True
                        End If
                    ElseIf DVScadAtt(idxRowDV).Item("Qta_Evasa") <> 0 Or txtDEV.Text.Trim <> "" Then
                        SWAgg = True
                    Else
                        If IsDBNull(DVScadAtt.Item(idxRowDV).Item("DataEv")) Then
                            'ok
                        ElseIf DVScadAtt.Item(idxRowDV).Item("DataEv").ToString.Trim <> "" Then
                            SWAgg = True
                        End If
                    End If
                    If chkFatturata.Checked Then
                        If DVScadAtt(idxRowDV).Item("Qta_Fatturata") = 0 Then
                            SWAgg = True
                        End If
                    ElseIf DVScadAtt(idxRowDV).Item("Qta_Fatturata") <> 0 Then
                        SWAgg = True
                    End If
                    If DVScadAtt(idxRowDV).Item("TextDataSc").ToString.Trim <> txtDSC.Text.Trim Then
                        SWAgg = True
                    End If
                    If IsDBNull(DVScadAtt(idxRowDV).Item("TextDataEv")) Then
                        Comodo = ""
                    Else
                        Comodo = DVScadAtt(idxRowDV).Item("TextDataEv").ToString.Trim
                    End If
                    If Comodo.Trim <> txtDEV.Text.Trim Then
                        SWAgg = True
                    End If
                    If IsDBNull(DVScadAtt(idxRowDV).Item("TextRefDataNC")) Then
                        Comodo = ""
                    Else
                        Comodo = DVScadAtt(idxRowDV).Item("TextRefDataNC").ToString.Trim
                    End If
                    If txtDSCNext.Text.Trim <> Comodo.Trim Then
                        SWAgg = True
                    End If
                End If
                idxRow += 1
            Next
        Catch ex As Exception
            SWErrore = True
            strErrore = ex.Message.Trim
        End Try
        If SWErrore Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiornamento Attività: ", "Data scadenza errata.<br>" + strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        If SWAgg = False Then
            AGGPAGScadAtt = True
            Exit Function
        End If
        '-------------------------------
        strErrore = ""
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
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataEV", System.Data.SqlDbType.DateTime, 8, "DataEV"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 8, "RefDataNC"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Fatturata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Fatturata", System.Data.DataRowVersion.Current, Nothing))
        If SqlConn.State <> ConnectionState.Open Then
            SqlConn.Open()
        End If
        '---------------------------
        idxRow = 0
        idxRowDV = 0
        For Each row As GridViewRow In GridViewDettCAAtt.Rows
            SWErrore = False
            chkEvasa = CType(row.FindControl("chkEvasa"), CheckBox)
            chkFatturata = CType(row.FindControl("chkFatturata"), CheckBox)
            txtDSC = CType(row.FindControl("txtDataSC"), TextBox)
            txtDEV = CType(row.FindControl("txtDataEv"), TextBox)
            '-
            If txtDSC.Text.Trim.Length <> 10 Then
                txtDSC.BackColor = SEGNALA_KO
                SWErrore = True
            ElseIf Not IsDate(txtDSC.Text.Trim) Then
                txtDSC.BackColor = SEGNALA_KO
                SWErrore = True
            Else
                txtDSC.BackColor = SEGNALA_OK
            End If
            If txtDEV.Text.Trim <> "" Then
                If Not IsDate(txtDEV.Text.Trim) Then
                    txtDEV.BackColor = SEGNALA_KO
                    SWErrore = True
                Else
                    txtDEV.BackColor = SEGNALA_OK
                End If
            Else
                txtDEV.BackColor = SEGNALA_OK
            End If
            'giu080223
            txtDSCNext = CType(row.FindControl("txtDataEVN"), TextBox)
            If txtDSCNext.Text.Trim <> "" Then
                If Not IsDate(txtDSCNext.Text.Trim) Then
                    txtDSCNext.BackColor = SEGNALA_KO
                    SWErrore = True
                Else
                    txtDSCNext.BackColor = SEGNALA_OK
                End If
            Else
                txtDSCNext.BackColor = SEGNALA_OK
            End If
            If SWErrore Then
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                Session(SWOP) = SWOPMODSCATT
                Session(SWOPMODSCATT) = SWSI
                GridViewDettCAAtt.Enabled = True
                btnModScAttCA.Visible = False
                btnAggScAttCA.Visible = True
                btnAnnScAttCA.Visible = True
                btnAggDataScAtt.Enabled = False
                btnAggFCFromOC.Enabled = False
                '-
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", "Data scadenza errata.", WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
            idxRowDV = idxRow + (GridViewDettCAAtt.PageSize * GridViewDettCAAtt.PageIndex)
            SWAgg = False
            If chkEvasa.Checked Then
                If DVScadAtt(idxRowDV).Item("Qta_Evasa") = 0 Or txtDEV.Text.Trim = "" Then
                    SWAgg = True
                End If
                If IsDBNull(DVScadAtt.Item(idxRowDV).Item("DataEv")) Then
                    SWAgg = True
                ElseIf DVScadAtt.Item(idxRowDV).Item("DataEv").ToString.Trim = "" Then
                    SWAgg = True
                End If
            ElseIf DVScadAtt(idxRowDV).Item("Qta_Evasa") <> 0 Or txtDEV.Text.Trim <> "" Then
                SWAgg = True
            Else
                If IsDBNull(DVScadAtt.Item(idxRowDV).Item("DataEv")) Then
                    'ok
                ElseIf DVScadAtt.Item(idxRowDV).Item("DataEv").ToString.Trim <> "" Then
                    SWAgg = True
                End If
            End If
            If chkFatturata.Checked Then
                If DVScadAtt(idxRowDV).Item("Qta_Fatturata") = 0 Then
                    SWAgg = True
                End If
            ElseIf DVScadAtt(idxRowDV).Item("Qta_Fatturata") <> 0 Then
                SWAgg = True
            End If
            If DVScadAtt(idxRowDV).Item("TextDataSc").ToString.Trim <> txtDSC.Text.Trim Then
                SWAgg = True
            End If
            If IsDBNull(DVScadAtt(idxRowDV).Item("TextDataEv")) Then
                Comodo = ""
            Else
                Comodo = DVScadAtt(idxRowDV).Item("TextDataEv").ToString.Trim
            End If
            If Comodo.Trim <> txtDEV.Text.Trim Then
                SWAgg = True
            End If
            If IsDBNull(DVScadAtt(idxRowDV).Item("TextRefDataNC")) Then
                Comodo = ""
            Else
                Comodo = DVScadAtt(idxRowDV).Item("TextRefDataNC").ToString.Trim
            End If
            If txtDSCNext.Text.Trim <> Comodo.Trim Then
                SWAgg = True
            End If
            If SWAgg = False Then
                idxRow += 1
                Continue For
            End If
            DVScadAtt.BeginInit()
            If chkEvasa.Checked Then
                DVScadAtt.Item(idxRowDV).Item("Qta_Selezionata") = DVScadAtt.Item(idxRowDV).Item("Qta_Ordinata")
                DVScadAtt.Item(idxRowDV).Item("Qta_Evasa") = DVScadAtt.Item(idxRowDV).Item("Qta_Ordinata")
                DVScadAtt.Item(idxRowDV).Item("Qta_Residua") = 0
                If txtDEV.Text.Trim = "" Then txtDEV.Text = Format(Now.Date, FormatoData)
            Else
                DVScadAtt.Item(idxRowDV).Item("Qta_Selezionata") = 0
                DVScadAtt.Item(idxRowDV).Item("Qta_Evasa") = 0
                DVScadAtt.Item(idxRowDV).Item("Qta_Residua") = DVScadAtt.Item(idxRowDV).Item("Qta_Ordinata")
                txtDEV.Text = ""
            End If
            DVScadAtt.Item(idxRowDV).Item("TextDataSc") = txtDSC.Text.Trim
            DVScadAtt.Item(idxRowDV).Item("TextDataEv") = txtDEV.Text.Trim
            DVScadAtt.Item(idxRowDV).Item("TextRefDataNC") = txtDSCNext.Text.Trim
            If chkFatturata.Checked Then
                DVScadAtt.Item(idxRowDV).Item("Qta_Fatturata") = DVScadAtt.Item(idxRowDV).Item("Qta_Ordinata")
            Else
                DVScadAtt.Item(idxRowDV).Item("Qta_Fatturata") = 0
            End If
            DVScadAtt.EndInit()
            If DVScadAtt.Item(idxRowDV).Item("Qta_Evasa") <> 0 Then SWOKEvasa = True
            Try
                SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = DVScadAtt.Item(idxRowDV).Item("IDDocumenti")
                SqlUpd_ConTScadPag.Parameters("@DurataNum").Value = DVScadAtt.Item(idxRowDV).Item("DurataNum")
                SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = DVScadAtt.Item(idxRowDV).Item("DurataNumRiga")
                SqlUpd_ConTScadPag.Parameters("@Riga").Value = DVScadAtt.Item(idxRowDV).Item("Riga")
                SqlUpd_ConTScadPag.Parameters("@Qta_Evasa").Value = DVScadAtt.Item(idxRowDV).Item("Qta_Evasa")
                SqlUpd_ConTScadPag.Parameters("@Qta_Residua").Value = DVScadAtt.Item(idxRowDV).Item("Qta_Residua")
                SqlUpd_ConTScadPag.Parameters("@DataSC").Value = CDate(DVScadAtt.Item(idxRowDV).Item("TextDataSc"))
                If IsDBNull(DVScadAtt.Item(idxRowDV).Item("TextDataEv")) Then
                    SqlUpd_ConTScadPag.Parameters("@DataEv").Value = DBNull.Value
                ElseIf DVScadAtt.Item(idxRowDV).Item("TextDataEv").ToString.Trim = "" Or Not IsDate(DVScadAtt.Item(idxRowDV).Item("TextDataEv").ToString.Trim) Then
                    SqlUpd_ConTScadPag.Parameters("@DataEv").Value = DBNull.Value
                Else
                    SqlUpd_ConTScadPag.Parameters("@DataEv").Value = CDate(DVScadAtt.Item(idxRowDV).Item("TextDataEv").ToString.Trim)
                End If
                'giu080223
                If IsDBNull(DVScadAtt.Item(idxRowDV).Item("TextRefDataNC")) Then
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                ElseIf DVScadAtt.Item(idxRowDV).Item("TextRefDataNC").ToString.Trim = "" Or Not IsDate(DVScadAtt.Item(idxRowDV).Item("TextRefDataNC").ToString.Trim) Then
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                Else
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = CDate(DVScadAtt.Item(idxRowDV).Item("TextRefDataNC").ToString.Trim)
                End If
                SqlUpd_ConTScadPag.Parameters("@Qta_Fatturata").Value = DVScadAtt.Item(idxRowDV).Item("Qta_Fatturata")
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
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            Finally
                'nulla
            End Try
            idxRow += 1
        Next
        If Not IsNothing(SqlConn) Then
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
                SqlConn = Nothing
            End If
        End If
        AGGPAGScadAtt = True
    End Function
    'annullo tutte le modifiche
    Private Function AnnullaALLScadAtt() As Boolean
        AnnullaALLScadAtt = False
        If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session(CSTSCADATTCA)
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'controllo date scadenze sempre valide per tutte
        Dim strErrore As String = ""
        strErrore = ""
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
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataEV", System.Data.SqlDbType.DateTime, 8, "DataEV"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 8, "RefDataNC"))
        SqlUpd_ConTScadPag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Fatturata", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Qta_Fatturata", System.Data.DataRowVersion.Current, Nothing))
        If SqlConn.State <> ConnectionState.Open Then
            SqlConn.Open()
        End If
        '---------------------------
        Dim RowsAnnulla() As DataRow = DVScadAtt.Table.Select("")
        For Each rowAnnulla As DataRow In RowsAnnulla
            Try
                SqlUpd_ConTScadPag.Parameters("@IDDocumenti").Value = rowAnnulla.Item("IDDocumenti")
                SqlUpd_ConTScadPag.Parameters("@DurataNum").Value = rowAnnulla.Item("DurataNum")
                SqlUpd_ConTScadPag.Parameters("@DurataNumRiga").Value = rowAnnulla.Item("DurataNumRiga")
                SqlUpd_ConTScadPag.Parameters("@Riga").Value = rowAnnulla.Item("Riga")
                SqlUpd_ConTScadPag.Parameters("@Qta_Evasa").Value = rowAnnulla.Item("Qta_Evasa")
                SqlUpd_ConTScadPag.Parameters("@Qta_Residua").Value = rowAnnulla.Item("Qta_Residua")
                SqlUpd_ConTScadPag.Parameters("@DataSC").Value = CDate(rowAnnulla.Item("TextDataSc"))
                If IsDBNull(rowAnnulla.Item("DataEv")) Then
                    SqlUpd_ConTScadPag.Parameters("@DataEV").Value = DBNull.Value
                Else
                    SqlUpd_ConTScadPag.Parameters("@DataEV").Value = CDate(rowAnnulla.Item("DataEv"))
                End If
                '
                If IsDBNull(rowAnnulla.Item("TextRefDataNC")) Then
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                ElseIf rowAnnulla.Item("TextRefDataNC").ToString.Trim = "" Or Not IsDate(rowAnnulla.Item("TextRefDataNC").ToString.Trim) Then
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = DBNull.Value
                Else
                    SqlUpd_ConTScadPag.Parameters("@RefDataNC").Value = CDate(rowAnnulla.Item("TextRefDataNC").ToString.Trim)
                End If
                SqlUpd_ConTScadPag.Parameters("@Qta_Fatturata").Value = rowAnnulla.Item("Qta_Fatturata")
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
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("(Annulla Modifiche) Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            Catch ex As Exception
                If Not IsNothing(SqlConn) Then
                    If SqlConn.State <> ConnectionState.Closed Then
                        SqlConn.Close()
                        SqlConn = Nothing
                    End If
                End If
                strErrore = ex.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("(Annulla Modifiche) Errore in aggiornamento Attività: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            Finally
                'nulla
            End Try
        Next
        If Not IsNothing(SqlConn) Then
            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
                SqlConn = Nothing
            End If
        End If
        AnnullaALLScadAtt = True
    End Function
    Private Function SetDDLDettDurNumRiga() As Boolean
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then myID = "-1"
        '------------
        SetDDLDettDurNumRiga = True
        DDLDurNumRiga.Items.Clear()
        DDLDurNumRiga.Items.Add("[Tutti]")
        DDLDurNumRiga.Items(0).Value = -1
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsArt As New DataSet
        'giu190723 AND Riga=1"
        strSQL = "Select DurataNumRiga, Serie From ContrattiD WHERE IDDocumenti=" & myID.Trim & " AND DurataNum=0 GROUP BY DurataNumRiga, Serie ORDER BY Serie"
        Dim strComodo As String = ""
        Try
            Dim myRighe As Integer = 0
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, dsArt)
            If (dsArt.Tables.Count > 0) Then
                If (dsArt.Tables(0).Rows.Count > 0) Then
                    myRighe = dsArt.Tables(0).Rows.Count
                End If
            End If
            If myRighe > 0 Then
                For i = 0 To myRighe - 1
                    strComodo = IIf(IsDBNull(dsArt.Tables(0).Rows(i).Item("Serie")), "[????]", dsArt.Tables(0).Rows(i).Item("Serie"))
                    If strComodo <> "[????]" Then strComodo = FormattaNomeFile(strComodo) 'giu180723
                    DDLDurNumRiga.Items.Add((i + 1).ToString.Trim & " - " & strComodo)
                    DDLDurNumRiga.Items(i + 1).Value = i
                Next i
            Else
                DDLDurNumRiga.Items.Add("[Nessuna]")
                DDLDurNumRiga.Items(0).Value = 0
            End If
            DDLDurNumRiga.SelectedIndex = 0
        Catch Ex As Exception
            SetDDLDettDurNumRiga = False
        End Try
        'Periodo
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
        DDLPeriodo.Items.Clear()
        DDLPeriodo.Items.Add("[Tutti]")
        DDLPeriodo.Items(0).Value = -1
        If myDurataTipo.Trim = "M" Then
            For i = 0 To Val(myDurataNum) - 1
                DDLPeriodo.Items.Add(Format(CDate(myDataInizio), FormatoData))
                DDLPeriodo.Items(i + 1).Value = i
                myDataInizio = DateAdd(DateInterval.Month, 1, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "T" Then
            For i = 0 To Val(myDurataNum) - 1
                DDLPeriodo.Items.Add(Format(CDate(myDataInizio), FormatoData))
                DDLPeriodo.Items(i + 1).Value = i
                myDataInizio = DateAdd(DateInterval.Month, 3, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "Q" Then
            For i = 0 To Val(myDurataNum) - 1
                DDLPeriodo.Items.Add(Format(CDate(myDataInizio), FormatoData))
                DDLPeriodo.Items(i + 1).Value = i
                myDataInizio = DateAdd(DateInterval.Month, 4, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "S" Then
            For i = 0 To Val(myDurataNum) - 1
                DDLPeriodo.Items.Add(Format(CDate(myDataInizio), FormatoData))
                DDLPeriodo.Items(i + 1).Value = i
                myDataInizio = DateAdd(DateInterval.Month, 6, CDate(myDataInizio))
            Next i
        ElseIf myDurataTipo.Trim = "A" Then
            For i = 0 To Val(myDurataNum) - 1
                DDLPeriodo.Items.Add(Format(CDate(myDataInizio).Year, "0000"))
                DDLPeriodo.Items(i + 1).Value = i
                myDataInizio = DateAdd(DateInterval.Year, 1, CDate(myDataInizio))
            Next i
        Else 'non capitera' mai
            For i = 0 To Val(myDurataNum) - 1
                DDLPeriodo.Items.Add((i + 1).ToString.Trim & " - ????")
                DDLPeriodo.Items(i + 1).Value = i
            Next i
        End If
        DDLPeriodo.SelectedIndex = 0
    End Function

    Private Sub chkTutteLeDate_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteLeDate.CheckedChanged
        If chkTutteLeDate.Checked Then
            txtDataOLD.Text = "" : txtDataOLD.Enabled = False
            DDLPeriodo.Enabled = True
        Else
            txtDataOLD.Enabled = True
            DDLPeriodo.SelectedIndex = -1
            DDLPeriodo.Enabled = False
        End If
    End Sub

    Public Function VerificaDateCKCons(ByRef strSegnalaDateDiverse As String) As Boolean
        lblMessScadAtt.Text = ""
        Dim pMinDataCK As String = "" : Dim pMaxDataCK As String = "" : Dim pSWUnicaDataCK As Boolean = False
        '-----------------------
        If Not (Session(CSTSCADATTCA) Is Nothing) Then
            DVScadAtt = Session(CSTSCADATTCA)
            If DVScadAtt.Count > 0 Then
                'OK
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessuna Attività è presente.", WUC_ModalPopup.TYPE_ALERT)
                Exit Function
            End If
        Else
            lblTotaleAtt.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        'giu041123
        Dim NSL As String = ""
        '-
        Dim pCodVisita As String = "" : Dim pNVisite As Integer = 0
        If GetCodVisitaDATipoCAIdPag(pCodVisita, pNVisite) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore: ", "SESSIONE SCADENZE SCADUTA - Riprovate la modifica uscendo e rientrando. Definizione Tipo Contratto errato.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '---------
        Dim strErrore As String = ""
        DVScadAtt.RowFilter = ""
        Dim RowsAggiorna() As DataRow = DVScadAtt.Table.Select("Qta_Evasa=0", "DurataNumRiga,SerieLotto,DataSc")
        Dim strDataCKPrec As String = "" : Dim strDataConsPrec As String = "" : Dim strDNRPrec As String = "" : Dim strSeriePrec As String = ""
        strSegnalaDateDiverse = ""
        For Each rowAggiorna As DataRow In RowsAggiorna
            Try
                rowAggiorna.BeginEdit()
                rowAggiorna.Item("TextDataSc") = IIf(IsDBNull(rowAggiorna.Item("TextDataSc")), "", rowAggiorna.Item("TextDataSc"))
                rowAggiorna.EndEdit()
                'okcontrolli
                If strDNRPrec = "" Then
                    strDNRPrec = rowAggiorna.Item("DurataNumRiga")
                    strSeriePrec = rowAggiorna.Item("SerieLotto")
                    strDataCKPrec = "" : strDataConsPrec = ""
                    If IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                        '-
                    ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                        strDataCKPrec = rowAggiorna.Item("TextDataSc")
                    Else
                        strDataConsPrec = rowAggiorna.Item("TextDataSc")
                    End If
                    'prima volta
                    pSWUnicaDataCK = True
                    pMinDataCK = strDataCKPrec
                    pMaxDataCK = strDataCKPrec
                End If
                If strDNRPrec <> rowAggiorna.Item("DurataNumRiga") Or strSeriePrec <> rowAggiorna.Item("SerieLotto") Then
                    If IsDate(strDataCKPrec) And IsDate(strDataConsPrec) Then
                        If CDate(strDataCKPrec).Date > CDate(strDataConsPrec).Date Then
                            strSegnalaDateDiverse += "(" + strSeriePrec + ") Data Ver DAE superiore alla data del Consumabile " + strDataCKPrec + "-" + strDataConsPrec + "<br>"
                            pSWUnicaDataCK = False 'non sarà possibile di bilanciare a un unica data CK (la prima MIN)
                        ElseIf CDate(strDataCKPrec).Date <> CDate(strDataConsPrec).Date Then
                            strSegnalaDateDiverse += "(" + strSeriePrec + ") Data Ver DAE diversa alla data del Consumabile " + strDataCKPrec + "-" + strDataConsPrec + "<br>"
                        End If
                    End If
                    '-
                    If strSeriePrec <> rowAggiorna.Item("SerieLotto") And strDNRPrec = rowAggiorna.Item("DurataNumRiga") Then
                        If IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                            '-
                        ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                            If strDataCKPrec <> rowAggiorna.Item("TextDataSc") And strDataCKPrec <> "" Then
                                strSegnalaDateDiverse += "(" + strSeriePrec + ")(" + rowAggiorna.Item("SerieLotto").ToString.Trim + _
                                ") Date diverse tra N° Serie diversi.: " + strDataCKPrec + "-" + rowAggiorna.Item("TextDataSc").ToString.Trim + "<br>"
                            End If
                        End If
                    End If
                    strDNRPrec = rowAggiorna.Item("DurataNumRiga")
                    strSeriePrec = rowAggiorna.Item("SerieLotto").ToString.Trim
                    strDataCKPrec = "" : strDataConsPrec = "" : pMinDataCK = "" : pMaxDataCK = ""
                    If IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                        '-
                    ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                        strDataCKPrec = rowAggiorna.Item("TextDataSc").ToString.Trim
                    Else
                        strDataConsPrec = rowAggiorna.Item("TextDataSc").ToString.Trim
                    End If
                    'la MIN e MAX data CK NON PUO' MAI CAPITARE
                    '''If IsDate(pMinDataCK) And IsDate(strDataCKPrec) Then
                    '''    If CDate(pMinDataCK).Year <> CDate(strDataCKPrec).Year Then
                    '''        strDataCKPrec = Mid(strDataCKPrec, 1, 6) + Mid(pMinDataCK, 7, 4)
                    '''    End If
                    '''End If
                    '-
                    If IsDate(pMinDataCK) And IsDate(strDataCKPrec) Then
                        If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                            pMinDataCK = strDataCKPrec
                        ElseIf IsDate(pMaxDataCK) Then
                            If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                                pMaxDataCK = strDataCKPrec
                            End If
                        Else
                            pMaxDataCK = strDataCKPrec
                        End If
                    ElseIf IsDate(pMaxDataCK) And IsDate(strDataCKPrec) Then
                        If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                            pMaxDataCK = strDataCKPrec
                        End If
                    ElseIf IsDate(strDataCKPrec) Then
                        If IsDate(pMinDataCK) Then
                            If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                                pMinDataCK = strDataCKPrec
                            ElseIf IsDate(pMaxDataCK) Then
                                If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                                    pMaxDataCK = strDataCKPrec
                                End If
                            Else
                                pMaxDataCK = strDataCKPrec
                            End If
                        Else
                            pMinDataCK = strDataCKPrec
                        End If
                        '-
                        If IsDate(pMaxDataCK) Then
                            If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                                pMaxDataCK = strDataCKPrec
                            ElseIf IsDate(pMinDataCK) Then
                                If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                                    pMinDataCK = strDataCKPrec
                                End If
                            Else
                                pMinDataCK = strDataCKPrec
                            End If
                        Else
                            pMaxDataCK = strDataCKPrec
                        End If
                    End If
                Else
                    If IsDBNull(rowAggiorna.Item("Cod_Articolo")) Then
                        '-
                    ElseIf rowAggiorna.Item("Cod_Articolo").ToString.Trim = pCodVisita.Trim Then
                        If IsDate(strDataCKPrec) And IsDate(rowAggiorna.Item("TextDataSc")) Then
                            If CDate(strDataCKPrec).Date < CDate(rowAggiorna.Item("TextDataSc")).Date Then
                                strDataCKPrec = rowAggiorna.Item("TextDataSc").ToString.Trim
                            End If
                        Else
                            strDataCKPrec = rowAggiorna.Item("TextDataSc").ToString.Trim
                        End If
                    Else
                        If IsDate(strDataConsPrec) And IsDate(rowAggiorna.Item("TextDataSc")) Then
                            If CDate(strDataConsPrec).Date < CDate(rowAggiorna.Item("TextDataSc")).Date Then
                                strDataConsPrec = rowAggiorna.Item("TextDataSc").ToString.Trim
                            End If
                        Else
                            strDataConsPrec = rowAggiorna.Item("TextDataSc").ToString.Trim
                        End If
                    End If
                    'la MIN e MAX data CK
                    '''If IsDate(pMinDataCK) And IsDate(strDataCKPrec) Then
                    '''    If CDate(pMinDataCK).Year <> CDate(strDataCKPrec).Year Then
                    '''        strDataCKPrec = Mid(strDataCKPrec, 1, 6) + Mid(pMinDataCK, 7, 4)
                    '''    End If
                    '''End If
                    '-
                    If IsDate(pMinDataCK) And IsDate(strDataCKPrec) Then
                        If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                            pMinDataCK = strDataCKPrec
                        ElseIf IsDate(pMaxDataCK) Then
                            If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                                pMaxDataCK = strDataCKPrec
                            End If
                        Else
                            pMaxDataCK = strDataCKPrec
                        End If
                    ElseIf IsDate(pMaxDataCK) And IsDate(strDataCKPrec) Then
                        If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                            pMaxDataCK = strDataCKPrec
                        End If
                    ElseIf IsDate(strDataCKPrec) Then
                        If IsDate(pMinDataCK) Then
                            If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                                pMinDataCK = strDataCKPrec
                            ElseIf IsDate(pMaxDataCK) Then
                                If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                                    pMaxDataCK = strDataCKPrec
                                End If
                            Else
                                pMaxDataCK = strDataCKPrec
                            End If
                        Else
                            pMinDataCK = strDataCKPrec
                        End If
                        '-
                        If IsDate(pMaxDataCK) Then
                            If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                                pMaxDataCK = strDataCKPrec
                            ElseIf IsDate(pMinDataCK) Then
                                If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                                    pMinDataCK = strDataCKPrec
                                End If
                            Else
                                pMinDataCK = strDataCKPrec
                            End If
                        Else
                            pMaxDataCK = strDataCKPrec
                        End If
                    End If
                End If

            Catch ex As Exception
                strErrore = ex.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("(Verifica data Scadenza Attività) Errore: ", strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            Finally
                'nulla
            End Try
        Next
        'ultimo
        If IsDate(strDataCKPrec) And IsDate(strDataConsPrec) Then
            If CDate(strDataCKPrec).Date > CDate(strDataConsPrec).Date Then
                strSegnalaDateDiverse += "(" + strSeriePrec + ") Data Ver DAE superiore alla data del Consumabile " + strDataCKPrec + "-" + strDataConsPrec + "<br>"
                pSWUnicaDataCK = False 'non sarà possibile di bilanciare a un unica data CK (la prima MIN)
            ElseIf CDate(strDataCKPrec).Date <> CDate(strDataConsPrec).Date Then
                strSegnalaDateDiverse += "(" + strSeriePrec + ") Data Ver DAE diversa alla data del Consumabile " + strDataCKPrec + "-" + strDataConsPrec + "<br>"
            End If
            'la MIN e MAX data CK
            '''If CDate(pMinDataCK).Year <> CDate(strDataCKPrec).Year Then
            '''    strDataCKPrec = Mid(strDataCKPrec, 1, 6) + Mid(pMinDataCK, 7, 4)
            '''End If
            If IsDate(pMinDataCK) And IsDate(strDataCKPrec) Then
                If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                    pMinDataCK = strDataCKPrec
                ElseIf IsDate(pMaxDataCK) Then
                    If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                        pMaxDataCK = strDataCKPrec
                    End If
                Else
                    pMaxDataCK = strDataCKPrec
                End If
            ElseIf IsDate(pMaxDataCK) And IsDate(strDataCKPrec) Then
                If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                    pMaxDataCK = strDataCKPrec
                End If
            ElseIf IsDate(strDataCKPrec) Then
                If IsDate(pMinDataCK) Then
                    If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                        pMinDataCK = strDataCKPrec
                    ElseIf IsDate(pMaxDataCK) Then
                        If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                            pMaxDataCK = strDataCKPrec
                        End If
                    Else
                        pMaxDataCK = strDataCKPrec
                    End If
                Else
                    pMinDataCK = strDataCKPrec
                End If
                '-
                If IsDate(pMaxDataCK) Then
                    If CDate(strDataCKPrec) > CDate(pMaxDataCK) Then
                        pMaxDataCK = strDataCKPrec
                    ElseIf IsDate(pMinDataCK) Then
                        If CDate(strDataCKPrec) < CDate(pMinDataCK) Then
                            pMinDataCK = strDataCKPrec
                        End If
                    Else
                        pMinDataCK = strDataCKPrec
                    End If
                Else
                    pMaxDataCK = strDataCKPrec
                End If
            End If
        End If
        DVScadAtt.RowFilter = ""
        If DVScadAtt.Count > 0 Then DVScadAtt.Sort = "DataSc,Serie"
        If strSegnalaDateDiverse.Trim <> "" Then
            If Not IsDate(pMinDataCK) Or Not IsDate(pMaxDataCK) Then
                pMinDataCK = ""
                pMaxDataCK = ""
            ElseIf CDate(pMinDataCK).Year <> CDate(pMaxDataCK).Year And pSWUnicaDataCK = False Then
                pMinDataCK = ""
                pMaxDataCK = ""
            ElseIf Mid(pMinDataCK, 1, 5) = Mid(pMaxDataCK, 1, 5) And CDate(pMinDataCK).Year <> CDate(pMaxDataCK).Year Then
                pMinDataCK = ""
                pMaxDataCK = ""
                pSWUnicaDataCK = False
            End If
            '-
            Dim strUnicaDataCK As String = ""
            If pMinDataCK <> pMaxDataCK And pSWUnicaDataCK = False Then
                strUnicaDataCK = "<b>NON E' POSSIBILE L'ALLINEAMENTO DATE CHECK DATE</b><br>"
                '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                'ModalPopup.Show("Attenzione", "Controllare messaggi in sezione Attività", WUC_ModalPopup.TYPE_INFO)
                lblMessScadAtt.Text = "Attenzione<br>" + strUnicaDataCK + strSegnalaDateDiverse
            ElseIf pMinDataCK <> pMaxDataCK And pSWUnicaDataCK = True Then
                txtDataNEW.Text = pMinDataCK
                chkTutteLeDate.Checked = True
                DDLDurNumRiga.SelectedIndex = -1
                txtDataOLD.Enabled = False
                txtDataOLD.Text = ""
                chkNonAggConsum.Checked = False
                '-
                strUnicaDataCK = "<b>OK ALLINEAMENTO DATE CHECK DATE <br>alla data: " + pMinDataCK + " Max Data " + pMaxDataCK + "<br>Confermate L'ALLINEAMENTO DATE ?<br>alla data: " + pMinDataCK + "</b>"
                '''Session(MODALPOPUP_CALLBACK_METHOD) = "OKAggDataCKALL"
                '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                '''ModalPopup.Show("Attenzione - Non sarà aggiornata la data dei Consumabili", strUnicaDataCK + " " + Mid(strSegnalaDateDiverse, 1, 300), WUC_ModalPopup.TYPE_CONFIRM_YN)
                '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                'ModalPopup.Show("Attenzione", "Controllare messaggi in sezione Attività", WUC_ModalPopup.TYPE_INFO)
                lblMessScadAtt.Text = "Attenzione<br>" + strUnicaDataCK + strSegnalaDateDiverse
            ElseIf strSegnalaDateDiverse.Trim <> "" Then
                strUnicaDataCK = ""
                '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                '''ModalPopup.Show("Attenzione", strUnicaDataCK + " " + Mid(strSegnalaDateDiverse, 1, 300), WUC_ModalPopup.TYPE_INFO)
                '''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                '''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                'ModalPopup.Show("Attenzione", "Controllare messaggi in sezione Attività", WUC_ModalPopup.TYPE_INFO)
                lblMessScadAtt.Text = "Attenzione<br>" + strUnicaDataCK + strSegnalaDateDiverse
            End If

            Exit Function
        End If
    End Function
    Public Sub OKAggDataCKALL()
        Call btnAggDataScAtt_Click(Nothing, Nothing)
    End Sub

    Private Sub chkCTRFatturato_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkCTRFatturato.CheckedChanged
        lblTotaleFatturato.Text = ""
        '''lblTotaleFatturato.Visible = False
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
                    Dim myTotNettoPagare As String = "" : Dim myTotFatturato As Decimal = 0
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
                                myTotFatturato += CDec(myTotNettoPagare)
                            End If
                        Catch ex As Exception
                            myTotNettoPagare = "0"
                        End Try
                        '-
                        DDLCTRFatturato.Items.Add(Format(CDate(myDataFC), FormatoData) + " - " + myTotNettoPagare + " - (N°" + myNumFC.Trim + myFCPA + ")")
                        DDLCTRFatturato.Items(0).Value = row.Item("IDDocumenti")
                    Next
                    lblTotaleFatturato.Text = "Totale Fatture Emesse : " + (DDLCTRFatturato.Items.Count - 1).ToString.Trim + " per un Totale Fatturato di : " + Format(myTotFatturato, FormatoValEuro)
                    '''lblTotaleFatturato.Visible = True : lblMessRatePag.Visible = False
                    Try 'GIU211223
                        If CDec(lblTotFatturato.Text) <> CDec(myTotFatturato) Then
                            lblTotaleFatturato.ForeColor = Drawing.Color.DarkRed
                            lblTotFatturato.ForeColor = Drawing.Color.DarkRed
                        Else
                            lblTotaleFatturato.ForeColor = Drawing.Color.Blue
                            lblTotFatturato.ForeColor = Drawing.Color.Blue
                        End If
                    Catch ex As Exception

                    End Try
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

    Private Sub GridViewDettCAAtt_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDettCAAtt.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If CBool(e.Row.Cells(CellCAAtt.SWModAgenti).Text.Trim) = False Then
                e.Row.Cells(CellCAAtt.DataScCons).BackColor = SEGNALA_OKBTN
                e.Row.Cells(CellCAAtt.DataScCons).Enabled = False
            Else
                e.Row.Cells(CellCAAtt.DataScCons).BackColor = SEGNALA_OK
                e.Row.Cells(CellCAAtt.DataScCons).Enabled = True
            End If
        End If
    End Sub
End Class