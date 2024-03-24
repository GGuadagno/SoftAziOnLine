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
'Imports Microsoft.Reporting.WebForms
Imports System.IO 'giu140615

Partial Public Class WFP_EtichettePrepara
    Inherits System.Web.UI.UserControl
    Const CSTPosEtichetta As String = "PosIniEtichetta"
    'GIU210120
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup.WucElement = Me
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
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            BtnEtichettaA4.Visible = False
        Else
            BtnEtichettaA4.Visible = True
        End If
    End Sub

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property
    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        If PrimaVolta Then
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
            If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                BtnEtichettaA4.Visible = False
            Else
                BtnEtichettaA4.Visible = True
            End If
            '---
            Dim TmpIntest As New ArrayList
            Dim TmpItem As ListItem
            LnkStampa.Visible = False
            lblErrore.Visible = False
            '-
            If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                Session.Add(CSTPosEtichetta, 1)
                BtnEtichetta1.BackColor = Drawing.Color.Green
                BtnEtichettaA4.BackColor = Drawing.Color.Silver
            Else
                Session.Add(CSTPosEtichetta, 1)
                BtnEtichetta1.BackColor = Drawing.Color.Silver
                BtnEtichettaA4.BackColor = Drawing.Color.Green
            End If

            BtnEtichetta2.BackColor = Drawing.Color.Silver
            BtnEtichetta3.BackColor = Drawing.Color.Silver
            BtnEtichetta4.BackColor = Drawing.Color.Silver

            If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
                'in caso di gestione fattura accompagnatoria si dovrà prevedere la stampa...
                Dim DsPrinWebDoc As New DSPrintWeb_Documenti
                Dim CAPLocProvCompleto As String = ""
                Dim Tel1 As String = ""
                DsPrinWebDoc = Session(CSTDsPrepEtichette)
                If DsPrinWebDoc.Clienti.Rows.Count > 0 Then
                    If IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Filiale")) Then
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Rag_Soc").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.Clienti.Rows(0).Item("Rag_Soc")
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Denominazione").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.Clienti.Rows(0).Item("Denominazione")
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Indirizzo").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.Clienti.Rows(0).Item("Indirizzo")
                            TmpIntest.Add(TmpItem)
                        End If
                        CAPLocProvCompleto = DsPrinWebDoc.Clienti.Rows(0).Item("CAP").ToString.Trim
                        CAPLocProvCompleto = CAPLocProvCompleto & " " & DsPrinWebDoc.Clienti.Rows(0).Item("Localita").ToString.Trim
                        'giu270412 estero
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim <> "I" And _
                            DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim <> "IT" Then
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim <> "" Then
                                CAPLocProvCompleto = CAPLocProvCompleto & " (" & DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim & ")"
                            End If
                            If DsPrinWebDoc.Nazioni.Rows(0).Item("Descrizione").ToString.Trim <> "" Then
                                CAPLocProvCompleto = CAPLocProvCompleto & " " & DsPrinWebDoc.Nazioni.Rows(0).Item("Descrizione").ToString.Trim & " "
                            End If
                        Else
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Provincia").ToString.Trim <> "" Then
                                CAPLocProvCompleto = CAPLocProvCompleto & " (" & DsPrinWebDoc.Clienti.Rows(0).Item("Provincia").ToString.Trim & ")"
                            End If
                        End If
                        '----------------
                        If CAPLocProvCompleto <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = CAPLocProvCompleto
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.Clienti.Rows(0).Item("Telefono1").ToString.Trim <> "" Then
                            Tel1 = "Tel. " & DsPrinWebDoc.Clienti.Rows(0).Item("Telefono1").ToString.Trim
                        End If
                        If Tel1 <> "" Then
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Telefono2").ToString.Trim <> "" Then
                                Tel1 = Tel1 & DsPrinWebDoc.Clienti.Rows(0).Item("Telefono2").ToString.Trim
                            End If
                        Else
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Telefono2").ToString.Trim <> "" Then
                                Tel1 = "Tel. " & DsPrinWebDoc.Clienti.Rows(0).Item("Telefono2").ToString.Trim
                            End If
                        End If
                        If Tel1 <> "" Then
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Fax").ToString.Trim <> "" Then
                                Tel1 = Tel1 & " Fax " & DsPrinWebDoc.Clienti.Rows(0).Item("Fax").ToString.Trim
                            End If
                        Else
                            If DsPrinWebDoc.Clienti.Rows(0).Item("Fax").ToString.Trim <> "" Then
                                Tel1 = Tel1 & "Fax " & DsPrinWebDoc.Clienti.Rows(0).Item("Fax").ToString.Trim
                            End If
                        End If
                        If Tel1 <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = Tel1
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione2").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione2").ToString.Trim
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione3").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione3").ToString.Trim
                            TmpIntest.Add(TmpItem)
                        End If
                    Else
                        If DsPrinWebDoc.DestClienti.Rows.Count > 0 Then
                            If DsPrinWebDoc.DestClienti.Rows(0).Item("Ragione_Sociale35").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Ragione_Sociale35").ToString.Trim
                                TmpIntest.Add(TmpItem)
                            Else
                                If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim <> "" Then
                                    TmpItem = New ListItem
                                    TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1")
                                    TmpIntest.Add(TmpItem)
                                End If
                            End If
                        Else
                            If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1")
                                TmpIntest.Add(TmpItem)
                            End If
                        End If
                        '-
                        If DsPrinWebDoc.DestClienti.Rows.Count > 0 Then
                            If DsPrinWebDoc.DestClienti.Rows(0).Item("Denominazione").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Denominazione").ToString.Trim
                                TmpIntest.Add(TmpItem)
                            End If
                            If DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento35").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento35").ToString.Trim
                                TmpIntest.Add(TmpItem)
                            ElseIf DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento").ToString.Trim <> "" Then
                                TmpItem = New ListItem
                                TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento").ToString.Trim
                                TmpIntest.Add(TmpItem)
                            End If
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione2").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione2")
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione3").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione3")
                            TmpIntest.Add(TmpItem)
                        End If
                        Tel1 = ""
                        If DsPrinWebDoc.DestClienti.Rows.Count > 0 Then
                            If DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono1").ToString.Trim <> "" Then
                                Tel1 = "Tel. " & DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono1").ToString.Trim
                            End If
                            If Tel1 <> "" Then
                                If DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono2").ToString.Trim <> "" Then
                                    Tel1 = Tel1 & DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono2").ToString.Trim
                                End If
                            Else
                                If DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono2").ToString.Trim <> "" Then
                                    Tel1 = "Tel. " & DsPrinWebDoc.DestClienti.Rows(0).Item("Telefono2").ToString.Trim
                                End If
                            End If
                            If Tel1 <> "" Then
                                If DsPrinWebDoc.DestClienti.Rows(0).Item("Fax").ToString.Trim <> "" Then
                                    Tel1 = Tel1 & " Fax " & DsPrinWebDoc.DestClienti.Rows(0).Item("Fax").ToString.Trim
                                End If
                            Else
                                If DsPrinWebDoc.DestClienti.Rows(0).Item("Fax").ToString.Trim <> "" Then
                                    Tel1 = Tel1 & "Fax " & DsPrinWebDoc.DestClienti.Rows(0).Item("Fax").ToString.Trim
                                End If
                            End If
                        End If
                        If Tel1 <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = Tel1
                            TmpIntest.Add(TmpItem)
                        End If
                    End If

                    Dim NRiga As Integer = 0
                    For I = 0 To TmpIntest.Count - 1
                        Select Case I
                            Case 0
                                LblRiga1.Text = TmpIntest(I).Text
                            Case 1
                                LblRiga2.Text = TmpIntest(I).Text
                            Case 2
                                LblRiga3.Text = TmpIntest(I).Text
                            Case 3
                                LblRiga4.Text = TmpIntest(I).Text
                            Case 4
                                LblRiga5.Text = TmpIntest(I).Text
                            Case 5
                                LblRiga6.Text = TmpIntest(I).Text
                            Case 6
                                LblRiga7.Text = TmpIntest(I).Text
                            Case 7
                                LblRiga8.Text = TmpIntest(I).Text
                        End Select

                    Next I
                    LblDataDoc.Text = Format(CDate(DsPrinWebDoc.DocumentiT.Rows(0).Item("Data_Doc").ToString.Trim), "dd/MM/yyyy")
                    LblNumDoc.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Numero").ToString.Trim
                    If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Colli")) Then
                        txtNColli.Text = Int(DsPrinWebDoc.DocumentiT.Rows(0).Item("Colli").ToString.Trim)
                        If Int(txtNColli.Text) = 0 Then
                            Dim i As Integer = 0
                            Dim ncolliTmp As Integer = 0
                            For i = 0 To DsPrinWebDoc.DocumentiD.Rows.Count - 1
                                ncolliTmp = ncolliTmp + DsPrinWebDoc.DocumentiD.Rows(i).Item("Qta_Evasa")
                            Next i
                            txtNColli.Text = ncolliTmp
                        End If
                    Else
                        txtNColli.Text = 0
                    End If
                Else
                    Response.Redirect("..\Login.aspx?SessioneScaduta=1")
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
        End If

        ImgOrient.Src = "~\Immagini\Icone\Orizzontale.jpg"
        'lblMessUtente.Text = "Seleziona/modifica Quantità articoli da caricare"
        'WUCEvasioneOF.SelAndQtaTutti()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        'lblMessUtente.Text = valore
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        lblErrore.Visible = False
        Dim DsEtichette1 As New DSPrintWeb_Documenti
        Dim ObjReport As New Object

        If txtNColli.Text = "" Then
            lblErrore.Text = "La quantità dei colli è obbligatoria." : lblErrore.Visible = True
            Exit Sub
        End If

        If txtNColli.Text = "0" Then
            lblErrore.Text = "La quantità dei colli è obbligatoria." : lblErrore.Visible = True
            Exit Sub
        End If

        If Not IsNumeric(txtNColli.Text) Then
            lblErrore.Text = "Per la quantità dei colli è stato indicato un dato non valido." : lblErrore.Visible = True
            Exit Sub
        End If

        Try
            DsEtichette1 = Session(CSTDsPrepEtichette)
            DsEtichette1.EtichetteCollo.Clear()
            Dim RowEti As DSPrintWeb_Documenti.EtichetteColloRow
            Dim I As Integer = 0
            Dim K As Integer = 0
            Dim PosPartenza As Integer
            Dim PosFine As Integer
            Dim Ordine As Integer = 1
            Dim idxCollo As Integer = 1
            Dim DsPrinWebDoc As New DSPrintWeb_Documenti
            DsPrinWebDoc = Session(CSTDsPrepEtichette)
            PosPartenza = Session(CSTPosEtichetta)
            PosFine = PosPartenza + Int(txtNColli.Text)

            If BtnEtichettaA4.Visible And BtnEtichettaA4.BackColor = Drawing.Color.Green Then
                Ordine = 1
                idxCollo = 1
                PosPartenza = 1
            Else
                For K = 1 To PosPartenza - 1
                    RowEti = DsEtichette1.EtichetteCollo.NewRow
                    RowEti.Numero = Ordine
                    RowEti.Item("Riga1") = ""
                    RowEti.Item("Riga2") = ""
                    RowEti.Item("Riga3") = ""
                    RowEti.Item("Riga4") = ""
                    RowEti.Item("Riga5") = ""
                    RowEti.Item("Riga6") = ""
                    RowEti.Item("Riga7") = ""
                    RowEti.Item("Riga8") = ""
                    RowEti.Item("Riga9") = ""
                    RowEti.Desc_Collo = ""
                    RowEti.Desc_Spedizione = ""
                    DsEtichette1.EtichetteCollo.AddEtichetteColloRow(RowEti)
                    Ordine = Ordine + 1
                Next K
            End If

            Dim NRiga As Integer = 0
            For I = PosPartenza To PosFine - 1
                RowEti = DsEtichette1.EtichetteCollo.NewRow
                NRiga = 1
                RowEti.Numero = Ordine
                ''Pier 16/01/2012 allineato al Load, le label sono già pronte...
                RowEti.Riga1 = LblRiga1.Text
                RowEti.Riga2 = LblRiga2.Text
                RowEti.Riga3 = LblRiga3.Text
                RowEti.Riga4 = LblRiga4.Text
                RowEti.Riga5 = LblRiga5.Text
                RowEti.Riga6 = LblRiga6.Text
                RowEti.Riga7 = LblRiga7.Text
                RowEti.Riga8 = LblRiga8.Text
                RowEti.Desc_Documento = "Documento n. " & LblNumDoc.Text & " del " & LblDataDoc.Text   
                RowEti.Desc_Collo = "Collo " & idxCollo & " di " & Int(txtNColli.Text)
                RowEti.Desc_Spedizione = ""
                DsEtichette1.EtichetteCollo.AddEtichetteColloRow(RowEti)
                Ordine = Ordine + 1
                idxCollo = idxCollo + 1
            Next I
            'giu180320 OK
            Session(CSTObjReport) = ObjReport
            Session(CSTDsPrepEtichette) = DsEtichette1
            Session(CSTNOBACK) = 0
        Catch ex As Exception
            lblErrore.Text = "Errore in EtichettePrepara.btnStampa: " + ex.Message : lblErrore.Visible = True
            Exit Sub
        End Try
        Call OKApriStampa()
    End Sub

    Private Sub OKApriStampa()
        lblErrore.Visible = False
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDocST() = False Then
            lblErrore.Text = "Errore TIPO DOCUMENTO SCONOSCIUTO" : lblErrore.Visible = True
            Exit Sub
        End If
        '---------------------
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        NomeStampa = "ETICHETTE.PDF"
        SubDirDOC = "Ordini"

        Dim Rpt As Object = Nothing
        Rpt = New EtichetteCollo
        If BtnEtichettaA4.Visible And BtnEtichettaA4.BackColor = Drawing.Color.Green Then
            If CodiceDitta = "01" Then
                Rpt = New EtichetteColloA405
            ElseIf CodiceDitta = "05" Then
                Rpt = New EtichetteColloA405
            ElseIf CodiceDitta = "0501" Then
                Rpt = New EtichetteColloA405
            ElseIf CodiceDitta = "0502" Then
                Rpt = New EtichetteColloA405
            End If
        Else
            If CodiceDitta = "01" Then
                Rpt = New EtichetteCollo01
            ElseIf CodiceDitta = "05" Then
                Rpt = New EtichetteCollo05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New EtichetteCollo0501
            ElseIf CodiceDitta = "0502" Then
                Rpt = New EtichetteCollo0502
            End If
        End If

        Dim DsEtichette1 As New DSPrintWeb_Documenti
        DsEtichette1 = Session(CSTDsPrepEtichette)
        Rpt.SetDataSource(DsEtichette1)
        Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
        'GIU240324
        '''Session(CSTESPORTAPDF) = True
        '''Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
        '''Dim stPathReport As String = Session(CSTPATHPDF)
        '''Try 'giu281112 errore che il file Ã¨ gia aperto
        '''    Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
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
        '''    lblErrore.Text = "Errore Esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message : lblErrore.Visible = True
        '''    Exit Sub
        '''End Try
        '''Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        '''LnkStampa.HRef = LnkName
        getOutputRPT(Rpt, "PDF")
        LnkStampa.Visible = True
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
    Public Function CKCSTTipoDocST(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDocST = True
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
                        Return True
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
        '------------------------------------------------------------
        'giu310112 codice ditta per la gestione delle stampe personalizzate
        CodiceDitta = Session(CSTCODDITTA)
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
    Private Sub Chiudi(ByVal strErrore As String)
        LnkStampa.Visible = False
        lblErrore.Visible = False
        Dim strRitorno As String = ""
        strRitorno = ""
        If strErrore.Trim = "" Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        Else
            strRitorno = "WF_Menu.aspx?labelForm=" & strErrore
        End If
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try

    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        LnkStampa.Visible = False
        lblErrore.Visible = False
        ProgrammaticModalPopup.Hide()
        Session(F_ETP_APERTA) = False
    End Sub

    Private Sub BtnEtichetta1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnEtichetta1.Click
        LnkStampa.Visible = False
        Session.Add(CSTPosEtichetta, 1)
        BtnEtichetta1.BackColor = Drawing.Color.Green
        BtnEtichetta2.BackColor = Drawing.Color.Silver
        BtnEtichetta3.BackColor = Drawing.Color.Silver
        BtnEtichetta4.BackColor = Drawing.Color.Silver
        BtnEtichettaA4.BackColor = Drawing.Color.Silver
    End Sub

    Private Sub BtnEtichetta2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnEtichetta2.Click
        LnkStampa.Visible = False
        Session.Add(CSTPosEtichetta, 2)
        BtnEtichetta2.BackColor = Drawing.Color.Green
        BtnEtichetta1.BackColor = Drawing.Color.Silver
        BtnEtichetta3.BackColor = Drawing.Color.Silver
        BtnEtichetta4.BackColor = Drawing.Color.Silver
        BtnEtichettaA4.BackColor = Drawing.Color.Silver
    End Sub

    Private Sub BtnEtichetta3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnEtichetta3.Click
        LnkStampa.Visible = False
        Session.Add(CSTPosEtichetta, 3)
        BtnEtichetta3.BackColor = Drawing.Color.Green
        BtnEtichetta1.BackColor = Drawing.Color.Silver
        BtnEtichetta2.BackColor = Drawing.Color.Silver
        BtnEtichetta4.BackColor = Drawing.Color.Silver
        BtnEtichettaA4.BackColor = Drawing.Color.Silver
    End Sub

    Private Sub BtnEtichetta4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnEtichetta4.Click
        LnkStampa.Visible = False
        Session.Add(CSTPosEtichetta, 4)
        BtnEtichetta4.BackColor = Drawing.Color.Green
        BtnEtichetta1.BackColor = Drawing.Color.Silver
        BtnEtichetta2.BackColor = Drawing.Color.Silver
        BtnEtichetta3.BackColor = Drawing.Color.Silver
        BtnEtichettaA4.BackColor = Drawing.Color.Silver
    End Sub
    'giu020822
    Private Sub BtnEtichettaA4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnEtichettaA4.Click
        LnkStampa.Visible = False
        Session.Add(CSTPosEtichetta, 1)
        BtnEtichetta4.BackColor = Drawing.Color.Silver
        BtnEtichetta1.BackColor = Drawing.Color.Silver
        BtnEtichetta2.BackColor = Drawing.Color.Silver
        BtnEtichetta3.BackColor = Drawing.Color.Silver
        BtnEtichettaA4.BackColor = Drawing.Color.Green
    End Sub
End Class