Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.Documenti
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient

Partial Public Class WUC_DocumentiSpeseTraspTot
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

    'giu100118 lo fà WUC_DOCUMENTI TUTTO COMMENTATO
    ' ''Private DsDocT As New DSDocumenti
    ' ''Private dvDocT As DataView

    ' ''Private SqlConnDoc As SqlConnection
    ' ''Private SqlAdapDoc As SqlDataAdapter
    ' ''Private SqlDbSelectCmd As SqlCommand
    ' ''Private SqlDbInserCmd As SqlCommand
    ' ''Private SqlDbUpdateCmd As SqlCommand
    ' ''Private SqlDbDeleteCmd As SqlCommand

    Private myTipoDoc As String

    Private ArrScadPag As ArrayList 'giu020321

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
        WFP_Vettori1.WucElement = Me

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSVettori.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
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
        'giu100118 lo fà gia WUC_Documenti
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
        'giu100118 lo fà WUC_DOCUMENTI 
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
        WFP_Vettori1.WucElement = Me
        If Session(F_ANAGRVETTORI_APERTA) = True Then
            WFP_Vettori1.Show()
        End If
    End Sub

    Public Function PopolaTxtDocTTD2(ByVal _dvDocT As DataView, ByRef strErrore As String) As Boolean
        Try

            'GIU100118
            Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            SqlDSVettori.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
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
            ' ''lblTotMerce.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Totale")), 0, _dvDocT.Item(0).Item("Totale"))
            ' ''lblTotMerce.Text = FormattaNumero(CDec(lblTotMerce.Text), Int(DecimaliVal))
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
            TxtDescrizioneImballo.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Descrizione_Imballo")), "", _dvDocT.Item(0).Item("Descrizione_Imballo"))
            'giu310320
            Try
                chkTotSpeseAdd0.AutoPostBack = False
                chkTotSpeseAdd0.Checked = CBool(IIf(IsDBNull(_dvDocT.Item(0).Item("TotSpeseAddebitate")), 0, _dvDocT.Item(0).Item("TotSpeseAddebitate")))
                If CDec(TxtSpeseIncasso.Text) > 0 Or CDec(TxtSpeseTrasporto.Text) > 0 Or CDec(TxtSpeseImballo.Text) > 0 Then
                    ' SpeseVarie QUI NON GESTITE
                Else
                    chkTotSpeseAdd0.Enabled = False
                End If
                If chkTotSpeseAdd0.Checked = True Then
                    chkTotSpeseAdd0.BackColor = SEGNALA_INFO
                Else
                    chkTotSpeseAdd0.BackColor = SEGNALA_OKLBL
                End If
                chkTotSpeseAdd0.AutoPostBack = True
            Catch ex As Exception
                chkTotSpeseAdd0.Enabled = False
                chkTotSpeseAdd0.BackColor = SEGNALA_KO
            End Try
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
            If IsDBNull(_dvDocT.Item(0).Item("Data_Scadenza_1")) Then
                lblDataScad1.Text = ""
            Else
                lblDataScad1.Text = Format(_dvDocT.Item(0).Item("Data_Scadenza_1"), FormatoData)
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Data_Scadenza_2")) Then
                lblDataScad2.Text = ""
            Else
                lblDataScad2.Text = Format(_dvDocT.Item(0).Item("Data_Scadenza_2"), FormatoData)
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Data_Scadenza_3")) Then
                lblDataScad3.Text = ""
            Else
                lblDataScad3.Text = Format(_dvDocT.Item(0).Item("Data_Scadenza_3"), FormatoData)
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Data_Scadenza_4")) Then
                lblDataScad4.Text = ""
            Else
                lblDataScad4.Text = Format(_dvDocT.Item(0).Item("Data_Scadenza_4"), FormatoData)
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Data_Scadenza_5")) Then
                lblDataScad5.Text = ""
            Else
                lblDataScad5.Text = Format(_dvDocT.Item(0).Item("Data_Scadenza_5"), FormatoData)
            End If
            'IMPORTO RATE DI SCADENZE RATE
            Dim TotRate As Decimal = 0
            If IsDBNull(_dvDocT.Item(0).Item("Rata_1")) Then
                lblImpRata1.Text = ""
            ElseIf _dvDocT.Item(0).Item("Rata_1") <> 0 Then
                lblImpRata1.Text = FormattaNumero(_dvDocT.Item(0).Item("Rata_1"), Int(DecimaliVal))
                TotRate += CDec(lblImpRata1.Text)
            Else
                lblImpRata1.Text = ""
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Rata_2")) Then
                lblImpRata2.Text = ""
            ElseIf _dvDocT.Item(0).Item("Rata_2") <> 0 Then
                lblImpRata2.Text = FormattaNumero(_dvDocT.Item(0).Item("Rata_2"), Int(DecimaliVal))
                TotRate += CDec(lblImpRata2.Text)
            Else
                lblImpRata2.Text = ""
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Rata_3")) Then
                lblImpRata3.Text = ""
            ElseIf _dvDocT.Item(0).Item("Rata_3") <> 0 Then
                lblImpRata3.Text = FormattaNumero(_dvDocT.Item(0).Item("Rata_3"), Int(DecimaliVal))
                TotRate += CDec(lblImpRata3.Text)
            Else
                lblImpRata3.Text = ""
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Rata_4")) Then
                lblImpRata4.Text = ""
            ElseIf _dvDocT.Item(0).Item("Rata_4") <> 0 Then
                lblImpRata4.Text = FormattaNumero(_dvDocT.Item(0).Item("Rata_4"), Int(DecimaliVal))
                TotRate += CDec(lblImpRata4.Text)
            Else
                lblImpRata4.Text = ""
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Rata_5")) Then
                lblImpRata5.Text = ""
            ElseIf _dvDocT.Item(0).Item("Rata_5") <> 0 Then
                lblImpRata5.Text = FormattaNumero(_dvDocT.Item(0).Item("Rata_5"), Int(DecimaliVal))
                TotRate += CDec(lblImpRata5.Text)
            Else
                lblImpRata5.Text = ""
            End If
            'giu020321
            'DATE DI SCADENZE RATE
            Dim NRate As Integer = 0
            Try
                If IsDBNull(_dvDocT.Item(0).Item("NoteNonEvasione")) Then
                    _dvDocT.Item(0).Item("NoteNonEvasione") = ""
                End If
                ArrScadPag = New ArrayList
                Dim myScad As ScadPagEntity
                If _dvDocT.Item(0).Item("NoteNonEvasione") <> "" Then
                    Dim lineaSplit As String() = _dvDocT.Item(0).Item("NoteNonEvasione").Split(";")
                    TotRate = 0
                    For i = 0 To lineaSplit.Count - 1
                        If lineaSplit(i).Trim <> "" And (i + 2) <= lineaSplit.Count - 1 Then

                            myScad = New ScadPagEntity
                            myScad.NRata = lineaSplit(i).Trim
                            i += 1
                            myScad.Data = lineaSplit(i).Trim
                            i += 1
                            myScad.Importo = lineaSplit(i).Trim
                            TotRate += CDec(myScad.Importo)
                            Select Case NRate
                                Case 0
                                    lblDataScad1.Text = myScad.Data
                                    lblImpRata1.Text = myScad.Importo
                                Case 1
                                    lblDataScad2.Text = myScad.Data
                                    lblImpRata2.Text = myScad.Importo
                                Case 2
                                    lblDataScad3.Text = myScad.Data
                                    lblImpRata3.Text = myScad.Importo
                                Case 3
                                    lblDataScad4.Text = myScad.Data
                                    lblImpRata4.Text = myScad.Importo
                                Case 4
                                    lblDataScad5.Text = myScad.Data
                                    lblImpRata5.Text = myScad.Importo
                                Case 5
                                    lblDataScad6.Text = myScad.Data
                                    lblImpRata6.Text = myScad.Importo
                                Case 6
                                    lblDataScad7.Text = myScad.Data
                                    lblImpRata7.Text = myScad.Importo
                                Case 7
                                    lblDataScad8.Text = myScad.Data
                                    lblImpRata8.Text = myScad.Importo
                                Case 8
                                    lblDataScad9.Text = myScad.Data
                                    lblImpRata9.Text = myScad.Importo
                                Case 9
                                    lblDataScad10.Text = myScad.Data
                                    lblImpRata10.Text = myScad.Importo
                                Case 10
                                    lblDataScad11.Text = myScad.Data
                                    lblImpRata11.Text = myScad.Importo
                                Case 11
                                    lblDataScad12.Text = myScad.Data
                                    lblImpRata12.Text = myScad.Importo
                            End Select
                            '-
                            ArrScadPag.Add(myScad)
                            NRate += 1
                        End If
                    Next
                End If
                'lblNumRate.Text = "Totale rate: " & FormattaNumero(NRate)
                Session(CSTSCADPAG) = ArrScadPag
            Catch ex As Exception
                NRate = 0
                'TotRate = 0
            End Try
            '---------
            LblTotaleRate.Text = FormattaNumero(TotRate, Int(DecimaliVal))
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
            'GIU180219
            If _dvDocT.Item(0).Item("IDAnagrProvv").ToString.Trim = "" Then
                Dim myCodCli As String = _dvDocT.Item(0).Item("Cod_Cliente").ToString
                If myCodCli.Trim <> "" Then
                    Dim mySplitIVA As Boolean = False
                    Call Documenti.CKClientiIPAByIDDocORCod(0, myCodCli.Trim, mySplitIVA, strErrore)
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
            If IsDBNull(_dvDocT.Item(0).Item("RitAcconto")) Then
                chkRitAcconto.AutoPostBack = False
                chkRitAcconto.Checked = False
                chkRitAcconto.AutoPostBack = True
            Else
                chkRitAcconto.AutoPostBack = False
                chkRitAcconto.Checked = _dvDocT.Item(0).Item("RitAcconto")
                chkRitAcconto.AutoPostBack = True
            End If
            '-
            If chkRitAcconto.Checked = False Then
                txtImponibileRA.Text = ""
                txtImponibileRA.Enabled = False
                txtPercRA.Text = ""
                txtPercRA.Enabled = False
                LblTotaleRA.Text = ""
            Else
                If IsDBNull(_dvDocT.Item(0).Item("ImponibileRA")) Then
                    txtImponibileRA.Text = FormattaNumero(0, Int(DecimaliVal))
                Else
                    txtImponibileRA.Text = FormattaNumero(_dvDocT.Item(0).Item("ImponibileRA"), Int(DecimaliVal))
                End If
                If IsDBNull(_dvDocT.Item(0).Item("PercRA")) Then
                    txtPercRA.Text = FormattaNumero(0, Int(DecimaliVal))
                Else
                    txtPercRA.Text = FormattaNumero(_dvDocT.Item(0).Item("PercRA"), Int(DecimaliVal))
                End If
                If IsDBNull(_dvDocT.Item(0).Item("TotaleRA")) Then
                    LblTotaleRA.Text = FormattaNumero(0, Int(DecimaliVal))
                Else
                    LblTotaleRA.Text = FormattaNumero(_dvDocT.Item(0).Item("TotaleRA"), Int(DecimaliVal))
                End If
            End If
            If IsDBNull(_dvDocT.Item(0).Item("TotNettoPagare")) Then
                LblTotNettoPagare.Text = FormattaNumero(0, Int(DecimaliVal))
            Else
                LblTotNettoPagare.Text = FormattaNumero(_dvDocT.Item(0).Item("TotNettoPagare"), Int(DecimaliVal))
            End If
            '---------
            '-Trasporto a mezzo
            DDLVettore1.Enabled = False : btnGestVett1.Enabled = False
            DDLVettore2.Enabled = False ': btnGestVett2.Enabled = False
            ' ''DDLVettore3.Enabled = False
            btnGestVett1.Enabled = False ': btnGestVett2.Enabled = False
            If IsDBNull(_dvDocT.Item(0).Item("Tipo_Spedizione")) Then
                optMittente.Checked = False
                optDestinatario.Checked = False
                optVettore.Checked = False
            ElseIf _dvDocT.Item(0).Item("Tipo_Spedizione").ToString.Trim = "M" Then
                optMittente.Checked = True
                optDestinatario.Checked = False
                optVettore.Checked = False
            ElseIf _dvDocT.Item(0).Item("Tipo_Spedizione").ToString.Trim = "D" Then
                optMittente.Checked = False
                optDestinatario.Checked = True
                optVettore.Checked = False
            ElseIf _dvDocT.Item(0).Item("Tipo_Spedizione").ToString.Trim = "V" Then
                optMittente.Checked = False
                optDestinatario.Checked = False
                optVettore.Checked = True
                If optVettore.Enabled = True Then
                    DDLVettore1.Enabled = True : btnGestVett1.Enabled = True
                    DDLVettore2.Enabled = True ': btnGestVett2.Enabled = True
                    ' ''DDLVettore3.Enabled = True
                    btnGestVett1.Enabled = True ': btnGestVett2.Enabled = True
                End If
            End If
            If Not IsDBNull(_dvDocT.Item(0).Item("Vettore_1")) Then
                PosizionaItemDDL(_dvDocT.Item(0).Item("Vettore_1").ToString.Trim, DDLVettore1)
                If optVettore.Enabled = True Then
                    DDLVettore1.Enabled = True : btnGestVett1.Enabled = True
                End If
            Else
                DDLVettore1.SelectedIndex = 0
            End If
            If Not IsDBNull(_dvDocT.Item(0).Item("Vettore_2")) Then
                PosizionaItemDDL(_dvDocT.Item(0).Item("Vettore_2").ToString.Trim, DDLVettore2)
                If optVettore.Enabled = True Then
                    DDLVettore2.Enabled = True ': btnGestVett2.Enabled = True
                End If
            Else
                DDLVettore2.SelectedIndex = 0
            End If
            ' ''If Not IsDBNull(_dvDocT.Item(0).Item("Vettore_3")) Then
            ' ''    PosizionaItemDDL(_dvDocT.Item(0).Item("Vettore_3").ToString.Trim, DDLVettore3)
            ' ''    DDLVettore3.Enabled = True
            ' ''End If
            'GIU071211
            If IsDBNull(_dvDocT.Item(0).Item("SWTipoEvSaldo")) Then
                optTipoEvSaldoSaldo.Checked = False
                optTipoEvSaldoParziale.Checked = False
            ElseIf _dvDocT.Item(0).Item("SWTipoEvSaldo") Then
                optTipoEvSaldoSaldo.Checked = True
                optTipoEvSaldoParziale.Checked = False
            Else
                optTipoEvSaldoSaldo.Checked = False
                optTipoEvSaldoParziale.Checked = True
            End If
            '-Porto Franco,Assegnato
            If IsDBNull(_dvDocT.Item(0).Item("Porto")) Then
                optFranco.Checked = False : optAssegnato.Checked = False
            ElseIf _dvDocT.Item(0).Item("Porto").ToString.Trim = "F" Then
                optFranco.Checked = True : optAssegnato.Checked = False
            ElseIf _dvDocT.Item(0).Item("Porto").ToString.Trim = "A" Then
                optFranco.Checked = False : optAssegnato.Checked = True
            End If
            TxtDescPorto.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("DescrizionePorto")), "", _dvDocT.Item(0).Item("DescrizionePorto"))
            '-
            TxtDescAspetto.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("Aspetto")), "", _dvDocT.Item(0).Item("Aspetto"))

            If IsDBNull(_dvDocT.Item(0).Item("Colli")) Then
                TxtNColli.Text = "0"
            Else
                TxtNColli.Text = FormattaNumero(_dvDocT.Item(0).Item("Colli"), 0)
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Pezzi")) Then
                TxtNPezzi.Text = "0"
            Else
                TxtNPezzi.Text = FormattaNumero(_dvDocT.Item(0).Item("Pezzi"), 0)
            End If
            If IsDBNull(_dvDocT.Item(0).Item("Peso")) Then
                TxtPesoKG.Text = FormattaNumero(0, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            Else
                TxtPesoKG.Text = FormattaNumero(_dvDocT.Item(0).Item("Peso"), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
            End If
            'Data,Ora Trasporto
            If IsDBNull(_dvDocT.Item(0).Item("DataOraRitiro")) Then
                TxtDataIniTrasp.Text = ""
                TxtOraIniTrasp.Text = ""
            Else
                TxtDataIniTrasp.Text = Format(_dvDocT.Item(0).Item("DataOraRitiro"), FormatoData)
                TxtOraIniTrasp.Text = Format(_dvDocT.Item(0).Item("DataOraRitiro"), FormatoOra)
                If TxtOraIniTrasp.Text.Trim = "00:00" Then TxtOraIniTrasp.Text = ""
                If TxtOraIniTrasp.Text.Trim = "00.00" Then TxtOraIniTrasp.Text = ""
            End If
            txtNoteRitiro.Text = IIf(IsDBNull(_dvDocT.Item(0).Item("NoteRitiro")), "", _dvDocT.Item(0).Item("NoteRitiro"))
        Catch ex As Exception
            Session(ERROREALL) = SWSI
            Session(ERRORE) = "(PopolaTxtDocTTD2) " & ex.Message.Trim
        End Try
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
        lblTotLordoMercePL.Text = ""
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
        'giu310320
        chkTotSpeseAdd0.Checked = False
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
        'DATE DI SCADENZE RATE
        lblDataScad1.Text = ""
        lblDataScad2.Text = ""
        lblDataScad3.Text = ""
        lblDataScad4.Text = ""
        lblDataScad5.Text = ""
        lblDataScad6.Text = ""
        lblDataScad7.Text = ""
        lblDataScad8.Text = ""
        lblDataScad9.Text = ""
        lblDataScad9.Text = ""
        lblDataScad10.Text = ""
        lblDataScad11.Text = ""
        lblDataScad12.Text = ""
        'IMPORTO RATE DI SCADENZE RATE
        lblImpRata1.Text = ""
        lblImpRata2.Text = ""
        lblImpRata3.Text = ""
        lblImpRata4.Text = ""
        lblImpRata5.Text = ""
        lblImpRata6.Text = ""
        lblImpRata7.Text = ""
        lblImpRata8.Text = ""
        lblImpRata9.Text = ""
        lblImpRata9.Text = ""
        lblImpRata10.Text = ""
        lblImpRata11.Text = ""
        lblImpRata12.Text = ""
        Session(CSTSCADPAG) = Nothing
        '-
        LblTotaleRate.Text = ""
        'giu211217 GIU120118
        'GIU0301219 SpliIVA arriva se richiesto da SESSIONE
        Try
            ChkSplitIVA.AutoPostBack = False
            ChkSplitIVA.Checked = Session(CSTSPLITIVA)
            ChkSplitIVA.AutoPostBack = True
        Catch ex As Exception
            ChkSplitIVA.AutoPostBack = False
            ChkSplitIVA.Checked = False
            Session(CSTSPLITIVA) = False
            ChkSplitIVA.AutoPostBack = True
        End Try
        '--------------------------------------------------
        txtImponibileRA.Text = ""
        txtPercRA.Text = ""
        LblTotaleRA.Text = ""
        LblTotNettoPagare.Text = ""
        chkRitAcconto.AutoPostBack = False
        chkRitAcconto.Checked = False
        chkRitAcconto.AutoPostBack = True
        '---------
        'BEGIN giu030112 richiesta di Cinzia DEFAULT OPTVettore
        '-Trasporto a mezzo 
        DDLVettore1.Enabled = True : DDLVettore2.Enabled = True ': DDLVettore3.Enabled = true
        btnGestVett1.Enabled = True ': btnGestVett2.Enabled = True
        DDLVettore1.SelectedIndex = 0 : DDLVettore2.SelectedIndex = 0 ': DDLVettore3.SelectedIndex = 0
        If DDLVettore1.Items.Count > 0 And DDLVettore1.Items.Count < 2 Then
            DDLVettore1.SelectedIndex = 1
        End If
        optMittente.AutoPostBack = False : optDestinatario.AutoPostBack = False : optVettore.AutoPostBack = False
        optMittente.Checked = False : optDestinatario.Checked = False : optVettore.Checked = True
        optMittente.AutoPostBack = True : optDestinatario.AutoPostBack = True : optVettore.AutoPostBack = True
        'END giu030112 richiesta di Cinzia DEFAULT OPTVettore
        optTipoEvSaldoSaldo.Checked = True
        optTipoEvSaldoParziale.Checked = False
        '-Porto Franco,Assegnato
        optFranco.AutoPostBack = False : optAssegnato.AutoPostBack = False
        optFranco.Checked = True : optAssegnato.Checked = False
        optFranco.AutoPostBack = True : optAssegnato.AutoPostBack = True
        TxtDescPorto.Text = "FRANCO"
        '-
        TxtDescAspetto.Text = GetParamGestAzi(Session(ESERCIZIO)).AspettoDeiBeni

        TxtNColli.Text = "0"
        TxtNPezzi.Text = "0"
        TxtPesoKG.Text = FormattaNumero(0, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Grandezze)
        'Data,Ora Trasporto
        TxtDataIniTrasp.Text = ""
        TxtOraIniTrasp.Text = ""
        txtNoteRitiro.Text = ""
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
        chkRitAcconto.BackColor = SEGNALA_OK
        txtImponibileRA.BackColor = SEGNALA_OK
        txtPercRA.BackColor = SEGNALA_OK
        LblTotaleRA.BackColor = SEGNALA_OKLBL
        LblTotNettoPagare.BackColor = SEGNALA_OKLBL
        LblTotaleRate.BackColor = SEGNALA_OKLBL
        '---------
        DDLVettore1.BackColor = SEGNALA_OK : DDLVettore2.BackColor = SEGNALA_OK ': DDLVettore3.BackColor = SEGNALA_OK
        TxtDescPorto.BackColor = SEGNALA_OK
        TxtDescAspetto.BackColor = SEGNALA_OK
        TxtNColli.BackColor = SEGNALA_OK
        TxtNPezzi.BackColor = SEGNALA_OK
        TxtPesoKG.BackColor = SEGNALA_OK
        TxtDataIniTrasp.BackColor = SEGNALA_OK
        TxtOraIniTrasp.BackColor = SEGNALA_OK
        txtNoteRitiro.BackColor = SEGNALA_OK
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
        chkTotSpeseAdd0.Enabled = Valore
        'giu211217
        ChkSplitIVA.Enabled = Valore
        txtImponibileRA.Enabled = Valore
        txtPercRA.Enabled = Valore
        chkRitAcconto.Enabled = Valore
        optMittente.Enabled = Valore
        optDestinatario.Enabled = Valore
        optVettore.Enabled = Valore
        optTipoEvSaldoSaldo.Enabled = Valore
        optTipoEvSaldoParziale.Enabled = Valore
        DDLVettore1.Enabled = Valore
        DDLVettore2.Enabled = Valore
        ' ''DDLVettore3.Enabled = Valore
        btnGestVett1.Enabled = Valore
        'btnGestVett2.Enabled = Valore
        optFranco.Enabled = Valore
        optAssegnato.Enabled = Valore
        TxtDescPorto.Enabled = Valore
        TxtDescAspetto.Enabled = Valore
        TxtNColli.Enabled = Valore
        TxtNPezzi.Enabled = Valore
        TxtPesoKG.Enabled = Valore
        TxtDataIniTrasp.Enabled = Valore
        imgBtnShowCalendar.Enabled = Valore
        TxtOraIniTrasp.Enabled = Valore
        txtNoteRitiro.Enabled = Valore
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
                            ByRef DataOraRitiro As String, _
                            ByRef NoteRitiro As String, _
                            ByRef CKSWTipoEvSaldo As Boolean, _
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

        For Cont = 0 To 12 'giu020321
            DataScad(Cont) = "" : ImpRata(Cont) = 0
        Next
        Try
            For Cont = 0 To 11 'giu020321
                Select Case Cont
                    Case 0
                        DataScad(Cont) = IIf(IsDate(lblDataScad1.Text.Trim), lblDataScad1.Text.Trim, "")
                        ImpRata(Cont) = IIf(lblImpRata1.Text.Trim <> "", CDec(lblImpRata1.Text.Trim), 0)
                    Case 1
                        DataScad(Cont) = IIf(IsDate(lblDataScad2.Text.Trim), lblDataScad2.Text.Trim, "")
                        ImpRata(Cont) = IIf(lblImpRata2.Text.Trim <> "", CDec(lblImpRata2.Text.Trim), 0)
                    Case 2
                        DataScad(Cont) = IIf(IsDate(lblDataScad3.Text.Trim), lblDataScad3.Text.Trim, "")
                        ImpRata(Cont) = IIf(lblImpRata3.Text.Trim <> "", CDec(lblImpRata3.Text.Trim), 0)
                    Case 3
                        DataScad(Cont) = IIf(IsDate(lblDataScad4.Text.Trim), lblDataScad4.Text.Trim, "")
                        ImpRata(Cont) = IIf(lblImpRata4.Text.Trim <> "", CDec(lblImpRata4.Text.Trim), 0)
                    Case 4
                        DataScad(Cont) = IIf(IsDate(lblDataScad5.Text.Trim), lblDataScad5.Text.Trim, "")
                        ImpRata(Cont) = IIf(lblImpRata5.Text.Trim <> "", CDec(lblImpRata5.Text.Trim), 0)
                    Case 5
                        DataScad(Cont) = IIf(IsDate(lblDataScad6.Text.Trim), lblDataScad6.Text.Trim, "")
                        ImpRata(Cont) += IIf(lblImpRata6.Text.Trim <> "", CDec(lblImpRata6.Text.Trim), 0)
                    Case 6
                        DataScad(Cont) = IIf(IsDate(lblDataScad7.Text.Trim), lblDataScad7.Text.Trim, "")
                        ImpRata(Cont) += IIf(lblImpRata7.Text.Trim <> "", CDec(lblImpRata7.Text.Trim), 0)
                    Case 7
                        DataScad(Cont) = IIf(IsDate(lblDataScad8.Text.Trim), lblDataScad8.Text.Trim, "")
                        ImpRata(Cont) += IIf(lblImpRata8.Text.Trim <> "", CDec(lblImpRata8.Text.Trim), 0)
                    Case 8
                        DataScad(Cont) = IIf(IsDate(lblDataScad9.Text.Trim), lblDataScad9.Text.Trim, "")
                        ImpRata(Cont) += IIf(lblImpRata9.Text.Trim <> "", CDec(lblImpRata9.Text.Trim), 0)
                    Case 9
                        DataScad(Cont) = IIf(IsDate(lblDataScad10.Text.Trim), lblDataScad10.Text.Trim, "")
                        ImpRata(Cont) += IIf(lblImpRata10.Text.Trim <> "", CDec(lblImpRata10.Text.Trim), 0)
                    Case 10
                        DataScad(Cont) = IIf(IsDate(lblDataScad11.Text.Trim), lblDataScad11.Text.Trim, "")
                        ImpRata(Cont) += IIf(lblImpRata11.Text.Trim <> "", CDec(lblImpRata11.Text.Trim), 0)
                    Case 11
                        DataScad(Cont) = IIf(IsDate(lblDataScad12.Text.Trim), lblDataScad12.Text.Trim, "")
                        ImpRata(Cont) += IIf(lblImpRata12.Text.Trim <> "", CDec(lblImpRata12.Text.Trim), 0)
                End Select
            Next
        Catch ex As Exception
        End Try
        'giu120118
        SplitIVA = ChkSplitIVA.Checked
        Session(CSTSPLITIVA) = SplitIVA
        RitAcconto = chkRitAcconto.Checked
        If RitAcconto = False Then
            ImponibileRA = 0 : txtImponibileRA.Text = ""
            PercRA = 0 : txtPercRA.Text = ""
            TotaleRA = 0 : LblTotaleRA.Text = ""
        Else
            Try
                If txtImponibileRA.Text.Trim = "" Then txtImponibileRA.Text = "0"
                ImponibileRA = CDec(txtImponibileRA.Text.Trim)
            Catch ex As Exception
                ImponibileRA = 0
                GetDati = False : txtImponibileRA.BackColor = SEGNALA_KO
            End Try
            Try
                If txtPercRA.Text.Trim = "" Then txtPercRA.Text = "0"
                PercRA = CDec(txtPercRA.Text.Trim)
            Catch ex As Exception
                PercRA = 0
                GetDati = False : txtPercRA.BackColor = SEGNALA_KO
            End Try
            Try
                If LblTotaleRA.Text.Trim = "" Then LblTotaleRA.Text = "0"
                TotaleRA = CDec(LblTotaleRA.Text.Trim)
            Catch ex As Exception
                TotaleRA = 0
                GetDati = False : txtImponibileRA.BackColor = SEGNALA_KO : txtPercRA.BackColor = SEGNALA_KO : LblTotaleRA.BackColor = SEGNALA_KO
            End Try
            'giu160118
            If ImponibileRA = 0 Then txtImponibileRA.BackColor = SEGNALA_KO
            If PercRA = 0 Then txtPercRA.BackColor = SEGNALA_KO
            If TotaleRA = 0 Then txtImponibileRA.BackColor = SEGNALA_KO : txtPercRA.BackColor = SEGNALA_KO : LblTotaleRA.BackColor = SEGNALA_KO
        End If
        '-
        Try
            If LblTotNettoPagare.Text.Trim = "" Then LblTotNettoPagare.Text = "0"
            TotNettoPagare = CDec(LblTotNettoPagare.Text.Trim)
            'giu230218 Controllo in base al documenti (Zibordi)
            If myTipoDoc = SWTD(TD.CaricoMagazzino) Or _
               myTipoDoc = SWTD(TD.ScaricoMagazzino) Or _
               myTipoDoc = SWTD(TD.MovimentoMagazzino) Then
                'NON OBBLIGATORIO
            Else
                'GIU260419
                If LblTotDocumento.Text.Trim = "" Then LblTotDocumento.Text = "0"
                If lblTotDeduzioni.Text.Trim = "" Then lblTotDeduzioni.Text = "0"
                If lblTotLordoMerce.Text.Trim = "" Then lblTotLordoMerce.Text = "0" 'GIU290519
                If TotNettoPagare = 0 And (CDec(LblTotDocumento.Text) = 0 Or CDec(LblTotDocumento.Text) < 0) And CDec(lblTotDeduzioni.Text) = 0 And CDec(lblTotLordoMerce.Text) = 0 Then
                    GetDati = False
                    LblTotDocumento.BackColor = SEGNALA_KO
                    LblTotNettoPagare.BackColor = SEGNALA_KO
                End If
            End If
        Catch ex As Exception
            TotNettoPagare = 0
            GetDati = False : LblTotNettoPagare.BackColor = SEGNALA_KO
        End Try
        '---------
        TipoSped = "M"
        If optMittente.Checked Then TipoSped = "M"
        If optDestinatario.Checked Then TipoSped = "D"
        If optVettore.Checked Then TipoSped = "V"
        Vettori(0) = IIf(DDLVettore1.SelectedIndex > 0, DDLVettore1.SelectedValue, 0)
        Vettori(1) = IIf(DDLVettore2.SelectedIndex > 0, DDLVettore2.SelectedValue, 0)
        ' ''Vettori(2) = IIf(DDLVettore3.SelectedIndex <> 0, DDLVettore3.SelectedValue, 0)
        If optTipoEvSaldoSaldo.Checked Then
            CKSWTipoEvSaldo = True
        Else
            CKSWTipoEvSaldo = False
        End If
        If optFranco.Checked Then Porto = "F" : If optAssegnato.Checked Then Porto = "A"
        DesPorto = TxtDescPorto.Text.Trim
        Aspetto = TxtDescAspetto.Text.Trim
        Try
            If TxtNColli.Text.Trim = "" Then TxtNColli.Text = "0"
            Colli = CLng(TxtNColli.Text)
        Catch ex As Exception
            Colli = 0
            GetDati = False : TxtNColli.BackColor = SEGNALA_KO
        End Try
        Try
            If TxtNPezzi.Text.Trim = "" Then TxtNPezzi.Text = "0"
            Pezzi = CLng(TxtNPezzi.Text)
        Catch ex As Exception
            Pezzi = 0
            GetDati = False : TxtNPezzi.BackColor = SEGNALA_KO
        End Try
        Try
            If TxtPesoKG.Text.Trim = "" Then TxtPesoKG.Text = "0"
            Peso = CDec(TxtPesoKG.Text)
        Catch ex As Exception
            Peso = 0
            GetDati = False : TxtPesoKG.BackColor = SEGNALA_KO
        End Try
        'GIU040112 NON MEMORIZZA DATA E ORA SE HO SCRITTO SOLO 12 E NON 12.00
        Dim OraRitiro As String = ""
        Try
            '-- controllo data
            If TxtDataIniTrasp.Text.Trim <> "" Then
                TxtDataIniTrasp.Text = Format(CDate(TxtDataIniTrasp.Text.Trim), FormatoData)
            End If
        Catch ex As Exception
            TxtDataIniTrasp.Text = Format(Now.Date, FormatoData)
        End Try
        Try
            '-- controllo ora
            If TxtOraIniTrasp.Text.Trim <> "" Then
                OraRitiro = TxtOraIniTrasp.Text.Trim.Replace(".", ":")
                If OraRitiro.Length < 3 Then
                    OraRitiro += ":00"
                End If
                If Not IsDate(CDate(Now.Date & " " & OraRitiro)) Then
                    OraRitiro = Format(Now.Hour, "00") & ":" & Format(Now.Minute, "00")
                End If
            End If
        Catch ex As Exception
            TxtOraIniTrasp.Text = "00:00" 'giu160612 Format(Now.Hour, "00") & ":" & Format(Now.Minute, "00")
            OraRitiro = "00:00" 'giu160612 Format(Now.Hour, "00") & ":" & Format(Now.Minute, "00")
        End Try
        '------
        Try
            DataOraRitiro = ""
            '--------------------------------------------------------------------
            If TxtDataIniTrasp.Text.Trim <> "" And OraRitiro.Trim <> "" Then
                DataOraRitiro = Format(CDate(TxtDataIniTrasp.Text.Trim & " " & OraRitiro.Trim.Replace(".", ":")), FormatoDataOra)
            ElseIf TxtDataIniTrasp.Text.Trim <> "" And OraRitiro.Trim = "" Then
                OraRitiro = "00:00" 'giu160612 Format(Now.Hour, "00") & ":" & Format(Now.Minute, "00")
                DataOraRitiro = Format(CDate(TxtDataIniTrasp.Text.Trim & " " & OraRitiro), FormatoDataOra)
            End If
        Catch ex As Exception
            TxtDataIniTrasp.Text = Format(Now.Date, FormatoData)
            TxtOraIniTrasp.Text = "00:00" 'giu160612 Format(Now.Hour, "00") & ":" & Format(Now.Minute, "00")
            OraRitiro = "00:00" 'giu160612 Format(Now.Hour, "00") & ":" & Format(Now.Minute, "00")
            DataOraRitiro = Format(CDate(TxtDataIniTrasp.Text.Trim & " " & OraRitiro), FormatoDataOra)
        End Try
        NoteRitiro = txtNoteRitiro.Text.Trim
    End Function
    Public Function CalcolaTotSpeseScad(ByVal dsDocDett As DataSet, ByRef Iva() As Integer, ByRef Imponibile() As Decimal, ByRef Imposta() As Decimal, ByVal DecimaliValuta As Integer, _
                                        ByRef MoltiplicatoreValuta As Integer, ByRef Totale As Decimal, ByRef TotaleLordoMerce As Decimal, ByVal ScontoCassa As Decimal, _
                                        Optional ByVal Listino As Long = 9999, Optional ByVal TipoDocumento As String = "DT", Optional ByVal Abbuono As Decimal = 0, _
                                        Optional ByRef strErrore As String = "", Optional ByRef TotaleLordoMercePL As Decimal = 0, _
                                        Optional ByRef Deduzioni As Decimal = 0) As Boolean 'giu020519
        'TD3 SPESE,TRASPORTO, TOTALE AGGIORNARE
        Call SfondoCampiDocTTD2() 'GIU260419
        Dim Cont As Integer
        lblTotMerce.Text = FormattaNumero(Totale, DecimaliValuta)
        lblTotLordoMerce.Text = FormattaNumero(TotaleLordoMerce, DecimaliValuta)
        lblTotLordoMercePL.Text = FormattaNumero(TotaleLordoMercePL, DecimaliValuta)
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
        If chkRitAcconto.Checked = True And txtImponibileRA.Text <> "" And txtPercRA.Text <> "" Then
            Dim myImp As Decimal = 0 : Dim myPerc As Decimal = 0
            Try
                myImp = CDec(txtImponibileRA.Text)
            Catch ex As Exception
                myImp = 0
            End Try
            Try
                myPerc = CDec(txtPercRA.Text)
            Catch ex As Exception
                myPerc = 0
            End Try
            If myImp <> 0 And myPerc <> 0 Then
                LblTotaleRA.Text = FormattaNumero(((myImp / 100) * myPerc), DecimaliValuta)
            Else
                LblTotaleRA.Text = ""
            End If
            '-
        Else
            LblTotaleRA.Text = ""
        End If
        '-
        Dim myTot As Decimal = 0
        Try
            myTot = CDec(LblTotaleRA.Text)
        Catch ex As Exception
            myTot = 0
        End Try
        If chkRitAcconto.Checked = True And myTot <> 0 Then
            LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotNettoPagare.Text) - myTot, DecimaliValuta)
        End If
        'giu130320 abbuono (arrotondamento) su totale documento
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
        If Calcola_Scadenze(TipoDocumento, DecimaliValuta, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Function
        End If

    End Function

    Private Function Calcola_Scadenze(ByVal TipoDocumento As String, ByVal DecimaliValuta As Integer, ByRef strErrore As String) As Boolean
        Calcola_Scadenze = True
        '' '' ''    'giu200510 DA FARE AL MOMENTO è DISABILITATO
        '' '' ''    If SWModScad = True Then
        '' '' ''        If MyTipoDoc = "FC" Or MyTipoDoc = "FT" Or MyTipoDoc = "NC" Or MyTipoDoc = "NZ" Then
        '' '' ''            '''cmdAggiornaScad.Visible = True: cmdAggiornaScad.Enabled = False
        '' '' ''        Else
        '' '' ''            SWChangeTipoPag = True
        '' '' ''        End If
        '' '' ''    End If
        '' '' ''    '----------
        '' '' ''    '''giu120410 giu140410
        '' '' ''    Dim TotRate As Currency
        '' '' ''    If SWModScad = True Then
        '' '' ''        If UCase(lblOperazione.caption) <> "NUOVO" And SWChangeTipoPag = False Then
        '' '' ''            Dim i As Integer : Dim ImpScad As Currency : TotRate = 0
        '' '' ''            For i = 1 To 5
        '' '' ''                ImpScad = IIf(IsNull(rsordiniT("Rata_" + Trim(i))), 0, rsordiniT("Rata_" + Trim(i)))
        '' '' ''                If ImpScad = 0 Then
        '' '' ''                    lblDataScadenza(i - 1).caption = ""
        '' '' ''                    lblImportoScadenza(i - 1).caption = ""
        '' '' ''                Else
        '' '' ''                    lblDataScadenza(i - 1).caption = IIf(IsNull(rsordiniT("Data_Scadenza_" + Trim(i))), "", Format(rsordiniT("Data_Scadenza_" + Trim(i)), "dd/mm/yyyy"))
        '' '' ''                    lblImportoScadenza(i - 1).caption = Formatta_Valute(ImpScad, DecimaliValutaFinito)
        '' '' ''                End If
        '' '' ''                TotRate = TotRate + ImpScad
        '' '' ''            Next i
        '' '' ''            lblTotRate.caption = Formatta_Valute(TotRate, DecimaliValutaFinito)
        '' '' ''            Exit Sub
        '' '' ''        End If
        '' '' ''    End If
        '' '' ''    SWChangeTipoPag = False
        '' '' ''    '---------
        '' '' ''    'giu200510 DA FARE AL MOMENTO è DISABILITATO
        'GIU010321 IL REDIM SARA' FATTO DOPO
        Dim Arr_Giorni() As String
        Dim Arr_Scad() As String
        Dim Arr_Impo() As Decimal
        'Dim NumDec As Integer adesso DecimaliValuta
        Dim NumRate As Integer = 0
        Dim Tot_Rate As Decimal = 0
        Dim ind As Integer = 0
        ' ''Dim RsTipoPag As recordset

        'DATE DI SCADENZE RATE
        lblDataScad1.Text = ""
        lblDataScad2.Text = ""
        lblDataScad3.Text = ""
        lblDataScad4.Text = ""
        lblDataScad5.Text = ""
        lblDataScad6.Text = ""
        lblDataScad7.Text = ""
        lblDataScad8.Text = ""
        lblDataScad9.Text = ""
        lblDataScad10.Text = ""
        lblDataScad11.Text = ""
        lblDataScad12.Text = ""
        'IMPORTO RATE DI SCADENZE RATE
        lblImpRata1.Text = ""
        lblImpRata2.Text = ""
        lblImpRata3.Text = ""
        lblImpRata4.Text = ""
        lblImpRata5.Text = ""
        lblImpRata6.Text = ""
        lblImpRata7.Text = ""
        lblImpRata8.Text = ""
        lblImpRata9.Text = ""
        lblImpRata10.Text = ""
        lblImpRata11.Text = ""
        lblImpRata12.Text = ""
        Session(CSTSCADPAG) = Nothing
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
            Calcola_Scadenze = False
            Exit Function
        End If
        'Codice Pagamento (da TD0)
        Dim CodPag As String = Session(CSTIDPAG)
        If IsNothing(CodPag) Then CodPag = ""
        If String.IsNullOrEmpty(CodPag) Then CodPag = ""
        If Not IsNumeric(CodPag) Then CodPag = ""
        If CodPag = "" Or CodPag = "0" Then Exit Function
        'RsTipoPag = DBCoGe.OpenRecordset("Pagamenti", dbOpenTable, dbReadOnly)
        'RsTipoPag.Index = "idx1"

        'RsTipoPag.Seek("=", lblCodPag.caption)
        'If RsTipoPag.NoMatch Then
        '    RsTipoPag.Close()
        '    RsTipoPag = Nothing
        '    Exit Function
        'End If
        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsPag As New DataSet
        Dim rowPag() As DataRow
        strSQL = "Select * From Pagamenti WHERE Codice = " & CodPag.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, dsPag)
            If (dsPag.Tables.Count > 0) Then
                If (dsPag.Tables(0).Rows.Count > 0) Then
                    rowPag = dsPag.Tables(0).Select()
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Codice pagamento nella tabella Pagamenti: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                    Calcola_Scadenze = False
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Codice pagamento nella tabella Pagamenti: " & CodPag.Trim, WUC_ModalPopup.TYPE_ERROR)
                Calcola_Scadenze = False
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura pagamenti: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End Try
        Comodo = IIf(IsDBNull(rowPag(0).Item("Numero_Rate")), "0", rowPag(0).Item("Numero_Rate"))
        If String.IsNullOrEmpty(Comodo) Then Comodo = "0"
        NumRate = CLng(Comodo)
        'GIU010321 Numero_Rate_Effettive
        Comodo = IIf(IsDBNull(rowPag(0).Item("Numero_Rate_Effettive")), "0", rowPag(0).Item("Numero_Rate_Effettive"))
        If String.IsNullOrEmpty(Comodo) Then Comodo = "0"
        If Not IsNumeric(Comodo) Then Comodo = "0"
        If CLng(Comodo) > 0 Then
            NumRate = CLng(Comodo)
        End If
        '-
        ReDim Arr_Giorni(NumRate)
        ReDim Arr_Impo(NumRate)
        ReDim Arr_Scad(NumRate)
        '-------------------------------
        'GIU090118 NON SERVE
        ' ''GIU030215 MODIFICA FATT.PA L'IVA DAL 2015 è VERSATA DAL CLIENTE 
        ' ''GIU300315 LA FATTURA PA NON è OBBLIGATORIO LO SPLIT IVA (BARBARA STUDIO MABELL)
        ' ''Dim SWFatturaPA As Boolean = False
        ' ''Dim mySWFatturaPA As String = Session(CSTFATTURAPA)
        ' ''If IsNothing(mySWFatturaPA) Then
        ' ''    SWFatturaPA = False
        ' ''End If
        ' ''If String.IsNullOrEmpty(mySWFatturaPA) Then
        ' ''    SWFatturaPA = False
        ' ''ElseIf Session(CSTFATTURAPA) Then
        ' ''    SWFatturaPA = True
        ' ''Else
        ' ''    SWFatturaPA = False
        ' ''End If
        '---------
        'giu160215
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            WucElement.Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
        'GIU090118 NON SERVE PERCHE' SE E' STATO CAMBIATO DAL CHK COMANDA LUI
        ' ''Dim SWSplitIVA As Boolean = False
        ' ''If Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", strErrore, SWSplitIVA) = True Then
        ' ''    If strErrore.Trim <> "" Then
        ' ''        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ' ''ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
        ' ''        ' ''Exit Sub
        ' ''        'non mi frega
        ' ''    End If
        ' ''End If
        '-
        ' ''If SWSplitIVA = True Then
        ' ''    If CompilaScadenze(rowPag, CDate(DataDoc), CDec(LblTotaleImpon.Text), _
        ' ''        DecimaliValuta, NumRate, Arr_Giorni, Arr_Scad, Arr_Impo, Tot_Rate, strErrore) = False Then
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Errore", "Nella compilazione Scadenze" & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, ""), WUC_ModalPopup.TYPE_ERROR)
        ' ''        Calcola_Scadenze = False
        ' ''        Exit Function
        ' ''    End If
        ' ''Else
        ' ''    If CompilaScadenze(rowPag, CDate(DataDoc), CDec(LblTotDocumento.Text), _
        ' ''        DecimaliValuta, NumRate, Arr_Giorni, Arr_Scad, Arr_Impo, Tot_Rate, strErrore) = False Then
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Errore", "Nella compilazione Scadenze" & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, ""), WUC_ModalPopup.TYPE_ERROR)
        ' ''        Calcola_Scadenze = False
        ' ''        Exit Function
        ' ''    End If
        ' ''End If
        If CompilaScadenze(rowPag, CDate(DataDoc), CDec(LblTotNettoPagare.Text), _
                DecimaliValuta, NumRate, Arr_Giorni, Arr_Scad, Arr_Impo, Tot_Rate, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nella compilazione Scadenze" & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, ""), WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End If
        '-
        Dim TotRate As Decimal = 0
        ArrScadPag = New ArrayList
        Dim myScad As New ScadPagEntity
        For ind = 0 To UBound(Arr_Scad) - 1
            myScad = New ScadPagEntity
            myScad.NRata = (ind + 1).ToString.Trim
            myScad.Data = Arr_Scad(ind)
            myScad.Importo = FormattaNumero(Arr_Impo(ind), DecimaliValuta)
            ArrScadPag.Add(myScad)
            Select Case ind
                Case 0
                    lblDataScad1.Text = Arr_Scad(ind)
                    lblImpRata1.Text = Arr_Impo(ind)
                    If lblImpRata1.Text.Trim <> "" Then
                        If CDec(lblImpRata1.Text.Trim) > 0 Then
                            lblImpRata1.Text = FormattaNumero(CDec(lblImpRata1.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata1.Text.Trim)
                        Else
                            lblImpRata1.Text = ""
                        End If
                    End If
                Case 1
                    lblDataScad2.Text = Arr_Scad(ind)
                    lblImpRata2.Text = Arr_Impo(ind)
                    If lblImpRata2.Text.Trim <> "" Then
                        If CDec(lblImpRata2.Text.Trim) > 0 Then
                            lblImpRata2.Text = FormattaNumero(CDec(lblImpRata2.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata2.Text.Trim)
                        Else
                            lblImpRata2.Text = ""
                        End If
                    End If
                Case 2
                    lblDataScad3.Text = Arr_Scad(ind)
                    lblImpRata3.Text = Arr_Impo(ind)
                    If lblImpRata3.Text.Trim <> "" Then
                        If CDec(lblImpRata3.Text.Trim) > 0 Then
                            lblImpRata3.Text = FormattaNumero(CDec(lblImpRata3.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata3.Text.Trim)
                        Else
                            lblImpRata3.Text = ""
                        End If
                    End If
                Case 3
                    lblDataScad4.Text = Arr_Scad(ind)
                    lblImpRata4.Text = Arr_Impo(ind)
                    If lblImpRata4.Text.Trim <> "" Then
                        If CDec(lblImpRata4.Text.Trim) > 0 Then
                            lblImpRata4.Text = FormattaNumero(CDec(lblImpRata4.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata4.Text.Trim)
                        Else
                            lblImpRata4.Text = ""
                        End If
                    End If
                Case 4
                    lblDataScad5.Text = Arr_Scad(ind)
                    lblImpRata5.Text = Arr_Impo(ind)
                    If lblImpRata5.Text.Trim <> "" Then
                        If CDec(lblImpRata5.Text.Trim) > 0 Then
                            lblImpRata5.Text = FormattaNumero(CDec(lblImpRata5.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata5.Text.Trim)
                        Else
                            lblImpRata5.Text = ""
                        End If
                    End If

                Case 5 'giu010321
                    lblDataScad6.Text = Arr_Scad(ind)
                    lblImpRata6.Text = Arr_Impo(ind)
                    If lblImpRata6.Text.Trim <> "" Then
                        If CDec(lblImpRata6.Text.Trim) > 0 Then
                            lblImpRata6.Text = FormattaNumero(CDec(lblImpRata6.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata6.Text.Trim)
                        Else
                            lblImpRata6.Text = ""
                        End If
                    End If
                Case 6
                    lblDataScad7.Text = Arr_Scad(ind)
                    lblImpRata7.Text = Arr_Impo(ind)
                    If lblImpRata7.Text.Trim <> "" Then
                        If CDec(lblImpRata7.Text.Trim) > 0 Then
                            lblImpRata7.Text = FormattaNumero(CDec(lblImpRata7.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata7.Text.Trim)
                        Else
                            lblImpRata7.Text = ""
                        End If
                    End If
                Case 7
                    lblDataScad8.Text = Arr_Scad(ind)
                    lblImpRata8.Text = Arr_Impo(ind)
                    If lblImpRata8.Text.Trim <> "" Then
                        If CDec(lblImpRata8.Text.Trim) > 0 Then
                            lblImpRata8.Text = FormattaNumero(CDec(lblImpRata8.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata8.Text.Trim)
                        Else
                            lblImpRata8.Text = ""
                        End If
                    End If
                Case 8
                    lblDataScad9.Text = Arr_Scad(ind)
                    lblImpRata9.Text = Arr_Impo(ind)
                    If lblImpRata9.Text.Trim <> "" Then
                        If CDec(lblImpRata9.Text.Trim) > 0 Then
                            lblImpRata9.Text = FormattaNumero(CDec(lblImpRata9.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata9.Text.Trim)
                        Else
                            lblImpRata9.Text = ""
                        End If
                    End If
                Case 9
                    lblDataScad10.Text = Arr_Scad(ind)
                    lblImpRata10.Text = Arr_Impo(ind)
                    If lblImpRata10.Text.Trim <> "" Then
                        If CDec(lblImpRata10.Text.Trim) > 0 Then
                            lblImpRata10.Text = FormattaNumero(CDec(lblImpRata10.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata10.Text.Trim)
                        Else
                            lblImpRata10.Text = ""
                        End If
                    End If
                Case 10
                    lblDataScad11.Text = Arr_Scad(ind)
                    lblImpRata11.Text = Arr_Impo(ind)
                    If lblImpRata11.Text.Trim <> "" Then
                        If CDec(lblImpRata11.Text.Trim) > 0 Then
                            lblImpRata11.Text = FormattaNumero(CDec(lblImpRata11.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata11.Text.Trim)
                        Else
                            lblImpRata11.Text = ""
                        End If
                    End If
                Case 11
                    lblDataScad12.Text = Arr_Scad(ind)
                    lblImpRata12.Text = Arr_Impo(ind)
                    If lblImpRata12.Text.Trim <> "" Then
                        If CDec(lblImpRata12.Text.Trim) > 0 Then
                            lblImpRata12.Text = FormattaNumero(CDec(lblImpRata12.Text.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata12.Text.Trim)
                        Else
                            lblImpRata12.Text = ""
                        End If
                    End If
                Case Else
                    TotRate = TotRate + Arr_Impo(ind)
            End Select
        Next ind
        LblTotaleRate.Text = FormattaNumero(TotRate, DecimaliValuta)
        Session(CSTSCADPAG) = ArrScadPag
        If CDec(LblTotNettoPagare.Text) <> TotRate Then
            LblTotaleRate.BackColor = SEGNALA_KO
            LblTotNettoPagare.BackColor = SEGNALA_KO
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Totale Rate scadenze diverso dal totale netto a pagare.", WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End If

    End Function

    Private Sub optVettore_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optVettore.CheckedChanged
        If optVettore.Checked = True Then
            DDLVettore1.Enabled = True : DDLVettore2.Enabled = True ': DDLVettore3.Enabled = True
            btnGestVett1.Enabled = True ': btnGestVett2.Enabled = True
        Else
            DDLVettore1.Enabled = False : DDLVettore2.Enabled = False ': DDLVettore3.Enabled = False
            DDLVettore1.SelectedIndex = 0 : DDLVettore2.SelectedIndex = 0 ': DDLVettore3.SelectedIndex = 0
            btnGestVett1.Enabled = False ': btnGestVett2.Enabled = False
        End If
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Sub optMittente_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optMittente.CheckedChanged
        If optMittente.Checked = True Then
            DDLVettore1.Enabled = False : DDLVettore2.Enabled = False ': DDLVettore3.Enabled = False
            DDLVettore1.SelectedIndex = 0 : DDLVettore2.SelectedIndex = 0 ': DDLVettore3.SelectedIndex = 0
            btnGestVett1.Enabled = False ': btnGestVett2.Enabled = False
        End If
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Sub optDestinatario_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optDestinatario.CheckedChanged
        If optDestinatario.Checked = True Then
            DDLVettore1.Enabled = False : DDLVettore2.Enabled = False ': DDLVettore3.Enabled = False
            DDLVettore1.SelectedIndex = 0 : DDLVettore2.SelectedIndex = 0 ': DDLVettore3.SelectedIndex = 0
            btnGestVett1.Enabled = False ': btnGestVett2.Enabled = False
        End If
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Sub optAssegnato_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optAssegnato.CheckedChanged
        If optAssegnato.Checked = True Then
            If TxtDescPorto.Text.Trim = "" Then
                TxtDescPorto.Text = "ASSEGNATO"
            ElseIf TxtDescPorto.Text.Trim = "FRANCO" Then
                TxtDescPorto.Text = "ASSEGNATO"
            End If
        End If
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Sub optFranco_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optFranco.CheckedChanged
        If optFranco.Checked = True Then
            If TxtDescPorto.Text.Trim = "" Then
                TxtDescPorto.Text = "FRANCO"
            ElseIf TxtDescPorto.Text.Trim = "ASSEGNATO" Then
                TxtDescPorto.Text = "FRANCO"
            End If
        End If
        Session(SWMODIFICATO) = SWSI
    End Sub

    Private Sub btnGestVett1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGestVett1.Click
        WFP_Vettori1.WucElement = Me
        WFP_Vettori1.SvuotaCampi()
        WFP_Vettori1.SetlblMessaggi("")
        Session(F_ANAGRVETTORI_APERTA) = True
        WFP_Vettori1.Show()
    End Sub
    Public Sub CallBackWFPAnagrVettori()
        Dim SAVEVettore1 As String = IIf(DDLVettore1.SelectedIndex > 0, DDLVettore1.SelectedValue, 0)
        Dim SAVEVettore2 As String = IIf(DDLVettore2.SelectedIndex > 0, DDLVettore2.SelectedValue, 0)

        Session(IDVETTORI) = ""
        Dim rk As StrVettori
        rk = Session(RKVETTORI)
        If IsNothing(rk.IDVettori) Then
            Exit Sub
        End If
        If IsNothing(rk.Descrizione) Then
            Exit Sub
        End If
        Session(IDVETTORI) = rk.IDVettori

        SqlDSVettori.DataBind()
        DDLVettore1.Items.Clear()
        DDLVettore1.Items.Add("")
        DDLVettore1.DataBind()
        '--
        DDLVettore2.Items.Clear()
        DDLVettore2.Items.Add("")
        DDLVettore2.DataBind()
        '-- mi riposiziono sul vettore/i
        PosizionaItemDDL(SAVEVettore1, DDLVettore1)
        PosizionaItemDDL(SAVEVettore2, DDLVettore2)
    End Sub
    Public Sub CancBackWFPAnagrVettori()

    End Sub

    'GIU040118 GIU110118
    Private Sub chkRitAcconto_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRitAcconto.CheckedChanged
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        Dim DecimaliValutaFinito As Integer = Int(DecimaliVal)
        '------------------------------------
        chkRitAcconto.BackColor = SEGNALA_OK
        txtImponibileRA.BackColor = SEGNALA_OK
        txtPercRA.BackColor = SEGNALA_OK
        LblTotaleRA.BackColor = SEGNALA_OKLBL
        If chkRitAcconto.Checked = True Then
            txtImponibileRA.Enabled = True
            txtPercRA.Enabled = True
            If txtImponibileRA.Text = "" Or txtImponibileRA.Text = "0" Then
                txtImponibileRA.Text = LblTotaleImpon.Text
            End If
            txtPercRA.Text = ""
            LblTotaleRA.Text = FormattaNumero(0, DecimaliValutaFinito)
        Else
            txtImponibileRA.Enabled = False
            txtPercRA.Enabled = False
            txtImponibileRA.Text = ""
            txtPercRA.Text = ""
            LblTotaleRA.Text = ""
        End If
        Call TotaleNettoPag()
        Session(SWMODIFICATO) = SWSI
        Dim strErrore As String = ""
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
        If Calcola_Scadenze(myTipoDoc, DecimaliValutaFinito, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Sub
        End If
        If chkRitAcconto.Checked = True Then
            If txtImponibileRA.Text <> "" Then
                txtPercRA.Focus()
            Else
                txtImponibileRA.Focus()
            End If
        End If
    End Sub
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
            Call Documenti.CKClientiIPAByIDDocORCod(0, myCodCli.Trim, mySplitIVA, strErrore)
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
        If Calcola_Scadenze(myTipoDoc, DecimaliValutaFinito, strErrore) = False Then
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
        If chkRitAcconto.Checked = True And txtImponibileRA.Text <> "" And txtPercRA.Text <> "" Then
            Dim myImp As Decimal = 0 : Dim myPerc As Decimal = 0
            Try
                myImp = CDec(txtImponibileRA.Text)
            Catch ex As Exception
                myImp = 0
            End Try
            Try
                myPerc = CDec(txtPercRA.Text)
            Catch ex As Exception
                myPerc = 0
            End Try
            If myImp <> 0 And myPerc <> 0 Then
                LblTotaleRA.Text = FormattaNumero(((myImp / 100) * myPerc), Int(DecimaliVal))
            Else
                LblTotaleRA.Text = ""
            End If
            '-
        Else
            LblTotaleRA.Text = ""
        End If
        '-
        Dim myTot As Decimal = 0
        Try
            myTot = CDec(LblTotaleRA.Text)
        Catch ex As Exception
            myTot = 0
        End Try
        If chkRitAcconto.Checked = True And myTot <> 0 Then
            LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotNettoPagare.Text) - myTot, Int(DecimaliVal))
        End If
        Try 'giu150320
            myTot = CDec(txtAbbuono.Text)
        Catch ex As Exception
            myTot = 0
        End Try
        LblTotNettoPagare.Text = FormattaNumero(CDec(LblTotNettoPagare.Text) + myTot, Int(DecimaliVal))
        '-
    End Sub

    Private Sub txtImponibileRA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtImponibileRA.TextChanged
        If chkRitAcconto.Checked = False Then
            Exit Sub
        End If
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
        If Calcola_Scadenze(myTipoDoc, DecimaliValutaFinito, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Sub
        End If
    End Sub
    Private Sub txtPercRA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPercRA.TextChanged
        If chkRitAcconto.Checked = False Then
            Exit Sub
        End If
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
        If Calcola_Scadenze(myTipoDoc, DecimaliValutaFinito, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Sub
        End If
    End Sub
    'giu180219 
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
    Public Function SetErrRitAcc(ByVal _OK As Boolean) As Boolean
        If _OK = False Then
            txtImponibileRA.BackColor = SEGNALA_KO
            txtPercRA.BackColor = SEGNALA_KO
            chkRitAcconto.BackColor = SEGNALA_KO
        Else
            txtImponibileRA.BackColor = SEGNALA_OK
            txtPercRA.BackColor = SEGNALA_OK
            chkRitAcconto.BackColor = SEGNALA_OK
        End If
    End Function

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

    Public Function SetLblTotLMPL(ByVal _TotLM As Decimal, ByVal _TotLMPL As Decimal, ByVal _Deduzioni As Decimal) As Boolean
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = "2"
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = "2"
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = "2" 'Euro
        End If
        '------------------------------------
        lblTotLordoMerce.Text = FormattaNumero(_TotLM, Int(DecimaliVal))
        lblTotLordoMercePL.Text = FormattaNumero(_TotLMPL, Int(DecimaliVal))
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
            'messaggio segnalato da WUC_Documenti _WucElement.Chiudi("Errore: Aggiornamento importi e scadenze")
            Exit Sub
        End If
        TxtSpeseIncasso.Focus()
    End Sub

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
        If Calcola_Scadenze(myTipoDoc, DecimaliValutaFinito, strErrore) = False Then
            If strErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                strErrore = ""
            End If
            Exit Sub
        End If
    End Sub

    Private Sub chkTotSpeseAdd0_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTotSpeseAdd0.CheckedChanged
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
            ModalPopup.Show("Errore in Contratti.chkTotSpeseAdd0", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim strSQL As String = ""
        If chkTotSpeseAdd0.Checked = True Then
            chkTotSpeseAdd0.BackColor = SEGNALA_INFO
            strSQL = "UPDATE DocumentiT SET TotSpeseAddebitate = -1 Where IDDocumenti=" & myID.Trim
        Else
            chkTotSpeseAdd0.BackColor = SEGNALA_OKLBL
            strSQL = "UPDATE DocumentiT SET TotSpeseAddebitate = 0 Where IDDocumenti=" & myID.Trim
        End If
        '============================================================================
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            Dim SWOk As Boolean = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL)
            If SWOk = False Then
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in Contratti.chkTotSpeseAdd0", "Si è verificato un errore durante l'aggiornamento testata TotSpeseAddebitate", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Contratti.chkTotSpeseAdd0", "Si è verificato un errore durante l'aggiornamento testata TotSpeseAddebitate. " & Ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        If chkTotSpeseAdd0.Checked = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Si ricorda che le spese non saranno riportate nei successivi documenti collegati a questo.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '============================================================================
    End Sub
End Class