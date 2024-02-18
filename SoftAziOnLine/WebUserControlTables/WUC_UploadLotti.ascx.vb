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

Partial Public Class WUC_UploadLotti
    Inherits System.Web.UI.UserControl

    Dim FileCaricato As Boolean

    Dim objDb As New DataBaseUtility
    Dim DsDocTDL As New DSDocumenti             'usato per caricare file csv, dettagli documento
    Dim DsLottiUpload As New DSDocumenti        'Usato per lotti insert e lotti già presenti in db

    Dim SqlConnDocDettL As SqlConnection
    Dim SqlAdapDocDettL As SqlDataAdapter
    Dim SqlDbInserCmdL As SqlCommand
    Dim strErrore As String
    Dim strRiepFinaleInsert As String
    Dim TipoDoc As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup1.WucElement = Me
    End Sub
    Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        Try
            If System.IO.File.Exists(Server.MapPath("~/Documenti/Temp/") + CStr(Trim(Session(CSTUTENTE))) + "_Lotti.csv") = True Then
                System.IO.File.Delete(Server.MapPath("~/Documenti/Temp/") + CStr(Trim(Session(CSTUTENTE))) + "_Lotti.csv")
            End If
        Catch
        End Try
    End Sub

#Region "Pulsanti"

    Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        btnAggiorna.Enabled = False
        btnUpload.Enabled = False
        FileUpload1.Enabled = False

        strRiepFinaleInsert = ""
        Dim Errore As Boolean = False

        DsDocTDL = Session.Item("dsCsvDettagliDoc")

        If DsDocTDL.Lotti.Count > 0 Then

            Call Me.SetCdmDAdp()
            DsLottiUpload.DocumentiDLottiInsert.Clear()
            DsLottiUpload.DocumentiDLotti.Clear()

            Dim stringaSelect As String = "SELECT * FROM DocumentiDLotti WHERE IDDocumenti=" & DsDocTDL.DocumentiT.Rows(0).Item("IDDocumenti").ToString.Trim
            objDb.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, stringaSelect, DsLottiUpload, "DocumentiDLotti")

            Dim strTitolo As String = ""
            Dim strMsg As String = ""

            For Each row As DataRow In DsDocTDL.Articoli.Rows
                Call Inserisci(row.Item("Cod_Articolo").ToString.Trim, strTitolo, strMsg)
                If Not strMsg = "" Then
                    Errore = True
                    Exit For
                End If
            Next
            If Errore = False Then
                If DsDocTDL.Lotti.Count <> DsLottiUpload.DocumentiDLottiInsert.Count Then
                    Errore = True
                    strTitolo = "Errore!"
                    strMsg = "Errore, inserimento lotti: <br>Differenza numero lotti da caricare con il numero lotti inseriti: </br> " & _
                    "Lotti da caricare: " & CStr(DsDocTDL.Lotti.Count) & "</br> Lotti da inserire: " & CStr(DsLottiUpload.DocumentiDLottiInsert.Count)
                End If
            End If
            If Errore = False Then
                Try
                    SqlAdapDocDettL.Update(DsLottiUpload.DocumentiDLottiInsert)
                Catch Ex As Exception
                    ' ''DsLottiUpload.DocumentiDLotti.Dispose()
                    ' ''DsLottiUpload.DocumentiDLottiInsert.Dispose()
                    DsLottiUpload.Dispose()
                    DsDocTDL.Dispose()
                    '---------
                    Errore = True
                    btnUpload.Enabled = True
                    FileUpload1.Enabled = True
                    '-
                    strTitolo = "Errore! (Sub btnAggiorna_Click)"
                    strMsg = "Errore, inserimento lotti: <br>" & Ex.Message.Trim
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup1.Show(strTitolo, strMsg, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
                'giu070516
                TipoDoc = Session.Item("TipoDoc")
                If TipoDoc = SWTD(TD.FatturaCommerciale) Or _
                   TipoDoc = SWTD(TD.DocTrasportoClienti) Or TipoDoc = SWTD(TD.FatturaAccompagnatoria) Or _
                   TipoDoc = SWTD(TD.CaricoMagazzino) Or _
                   TipoDoc = SWTD(TD.ScaricoMagazzino) Or _
                   TipoDoc = SWTD(TD.MovimentoMagazzino) Then
                    Session("GetScadProdCons") = ""
                    Dim SQLStr As String = ""
                    Try
                        SQLStr = "EXEC Carica_ArticoliInst_ContrattiAss " & DsDocTDL.DocumentiT.Rows(0).Item("IDDocumenti").ToString.Trim

                        If objDb.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr) = False Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup1.Show("Carico prodotti installati", "Errore, carico prodotti installati.", WUC_ModalPopup.TYPE_INFO)
                            Exit Sub
                        End If
                    Catch ex As Exception
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup1.Show("Carico prodotti installati", "Errore, carico prodotti installati.: " & ex.Message.Trim, WUC_ModalPopup.TYPE_INFO)
                        Exit Sub
                    End Try
                End If
                '-
                ' ''DsLottiUpload.DocumentiDLotti.Dispose()
                ' ''DsLottiUpload.DocumentiDLottiInsert.Dispose()
                DsLottiUpload.Dispose()
                DsDocTDL.Dispose()
                '---------
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup1.Show("Operazione completata", strRiepFinaleInsert, WUC_ModalPopup.TYPE_INFO)
            Else
                ' ''DsLottiUpload.DocumentiDLotti.Dispose()
                ' ''DsLottiUpload.DocumentiDLottiInsert.Dispose()
                DsLottiUpload.Dispose()
                DsDocTDL.Dispose()
                '---------
                btnUpload.Enabled = True
                FileUpload1.Enabled = True

                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup1.Show(strTitolo, strMsg, WUC_ModalPopup.TYPE_ERROR)
            End If

        Else
            lblFile.Text = "Nessun valore presente nel file csv caricato."
            btnUpload.Enabled = True
            FileUpload1.Enabled = True
        End If
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        If btnAggiorna.Enabled = True Then
            'RIMANGO QUI ALTRIMENTI RITORNO AL MENU
            FileUpload1.Enabled = True
            btnUpload.Enabled = True
            btnAggiorna.Enabled = False
            Try
                DsDocTDL.Dispose()
                DsLottiUpload.Dispose()
                Call Clear()
                lblFile.Text = ""
            Catch
            End Try
        Else
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
            Catch ex As Exception
                ModalPopup1.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            End Try
        End If
    End Sub

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click

        FileCaricato = False

        btnAggiorna.Enabled = False
        strErrore = ""
        '-
        Try
            'Fabio 18032016         -Se il file esiste lo cancello
            If System.IO.File.Exists(Server.MapPath("~/Documenti/Temp/") + CStr(Trim(Session(CSTUTENTE))) + "_Lotti.csv") = True Then
                System.IO.File.Delete(Server.MapPath("~/Documenti/Temp/") + CStr(Trim(Session(CSTUTENTE))) + "_Lotti.csv")
            End If
        Catch ex As Exception
            lblFile.Text = "Errore cancella file precedente: " & ex.Message.Trim
            Exit Sub
        End Try

        Try
            If FileUpload1.HasFile Then
                If FileUpload1.FileName.Substring(FileUpload1.FileName.Length - 4, 4) = ".csv" Then
                    Try
                        FileUpload1.SaveAs(Server.MapPath("~/Documenti/Temp/") + CStr(Trim(Session(CSTUTENTE))) + "_Lotti.csv")
                        lblFile.Text = "Nome del file: " + FileUpload1.PostedFile.FileName & "<br>" + "Dimensione del file: " & _
                           Math.Round(FileUpload1.PostedFile.ContentLength / 1024, 4) & " KB<br>" & _
                           "Tipo di file: " & _
                           FileUpload1.PostedFile.ContentType & ""
                        '-
                        Call Controlli()
                        If Not strErrore = "" Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup1.Show("Errore", strErrore, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        Else
                            FileCaricato = True
                        End If
                    Catch ex As Exception
                        lblFile.Text = "Errore Salva file e Controlli: " & ex.Message.Trim
                        Exit Sub
                    End Try
                    If FileCaricato = True Then
                        lblFile.Text = lblFile.Text + "<b></br></br>Controllo completato. Premere il tasto 'Aggiorna Lotti' per inserire i dati nel database.</b>"
                        FileUpload1.Enabled = True
                        btnAggiorna.Enabled = True
                    Else
                        lblFile.Text = "Errore nella verifica/elaborazione dei dati.: " & strErrore.Trim
                    End If
                Else
                    lblFile.Text = "<b>Errore: Il file non è stato caricato. Il formato è errato. <br>" & _
                    "Il file per essere elaborato deve essere con estensione '.csv'</b>"
                    Call Clear()
                End If
            Else
                lblFile.Text = "<b>Non è stato selezionato un file da caricare.</b>"
                Call Clear()
            End If
        Catch ex As Exception
            lblFile.Text = "Errore: " & ex.Message.Trim
            Exit Sub
        End Try

    End Sub

#End Region

#Region "Sub Functions"

    Sub CaricaDsArticoli() 'giu dsDocTDL (ByVal ds As DSDocumenti)
        Dim rowArt As DataRow
        DsDocTDL.Articoli.Clear()
        For Each row As DSDocumenti.LottiRow In DsDocTDL.Lotti.Rows
            'Carico DS ARTICOLI
            If DsDocTDL.Articoli.Select("Cod_Articolo='" & Controlla_Apice(row.Modello.Trim) & "'").Length = 0 Then
                rowArt = Nothing
                rowArt = DsDocTDL.Articoli.NewRow
                rowArt.Item(0) = row.Modello
                DsDocTDL.Articoli.Rows.Add(rowArt)
            End If
        Next
    End Sub
    Sub Controlli()

        strErrore = ""
        Dim strSQL As String = ""
        Dim CodCliente As String = ""
        Dim NumeroDoc As String
        Dim DataDoc As Date = Nothing
        Dim Anno As String = ""

        '-
        Dim dataAdpt As New SqlDataAdapter
        If RiempiDs() Then
            If DsDocTDL.Lotti.Count > 0 Then
                FileCaricato = True

                CodCliente = DsDocTDL.Lotti.Rows(0).Item(1)
                LblRiga1.Text = "Codice Cliente: " & CodCliente.Trim
                NumeroDoc = DsDocTDL.Lotti.Rows(0).Item("NumDDT")
                DataDoc = CDate(DsDocTDL.Lotti.Rows(0).Item("DataDDT"))
                LblDataDoc.Text = DsDocTDL.Lotti.Rows(0).Item("DataDDT").ToString.Trim
                
                TipoDoc = Session.Item("TipoDoc")
                LblNumDoc.Text = TipoDoc & " " & NumeroDoc.Trim

                strSQL = "SELECT * FROM DocumentiT "
                strSQL = strSQL + "WHERE (YEAR(DocumentiT.Data_Doc) = " + CStr(DataDoc.Year) + ") AND "
                strSQL = strSQL + "(DocumentiT.Numero = '" + NumeroDoc.Trim + "') AND (DocumentiT.Tipo_Doc = '" + TipoDoc.Trim + "' ) "
                objDb.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsDocTDL, "DocumentiT")
                If Not DsDocTDL.DocumentiT.Rows.Count > 0 Then
                    FileCaricato = False
                    strErrore = "Non è stato possibile trovare il documento all'interno dell'archivio.<br>Verificare che l'esercizio scelto sia lo stesso del documento in cui caricare i Lotti.<br>Oppure che i campi siano correttamente compilati."
                    Exit Sub
                ElseIf DsDocTDL.DocumentiT.Rows.Count > 1 Then
                    FileCaricato = False
                    strErrore = "Sono presenti più documenti all'interno dell'archivio con lo stesso numero."
                    Exit Sub
                End If

                Session(IDDOCUMENTI) = DsDocTDL.DocumentiT.Rows(0).Item("IDDocumenti")
                If RiempiDatiDoc(strErrore) = False Then
                    FileCaricato = False
                    Exit Sub
                End If

                Dim dataDocumentoDB As Date = DsDocTDL.DocumentiT.Rows(0).Item("Data_Doc") 'dsTemp.Tables(0).Rows(0).Item(2)

                If Not String.Equals(CodCliente, DsDocTDL.DocumentiT.Rows(0).Item("Cod_Cliente")) Then 'dsTemp.Tables(0).Rows(0).Item(1)) Then
                    FileCaricato = False
                    strErrore = "Il codice cliente indicato non corrisponde con il documento presente all'interno dell'archivio."
                ElseIf Not Date.Equals(Format(DataDoc, FormatoData), dataDocumentoDB.ToString(FormatoData)) Then
                    FileCaricato = False
                    strErrore = "La data del documento caricato, non corrisponde con quella del documento presente all'interno dell'archivio."
                End If

                If FileCaricato = True Then
                    'Controllo che tutti i cod cliente, numeroDoc e la data sia uguale per ogni record.
                    For Each row As DSDocumenti.LottiRow In DsDocTDL.Lotti.Rows
                        Try
                            If Not row.CodCliente.Trim = CodCliente.Trim Then
                                FileCaricato = False
                                strErrore = "I valori inerenti al 'Codice Cliente' nel documento caricato, non sono identici su tutte le righe."
                                Exit For
                            ElseIf Not CDate(row.DataDDT).Date = DataDoc.Date Then
                                FileCaricato = False
                                strErrore = "I valori inerenti alla 'Data Documento' nel documento caricato, non sono identici su tutte le righe."
                                Exit For
                            ElseIf Not row.NumDDT.Trim = NumeroDoc.Trim Then
                                FileCaricato = False
                                strErrore = "I valori inerenti al 'Numero Documento' nel documento caricato, non sono identici su tutte le righe."
                                Exit For
                            ElseIf String.IsNullOrEmpty(row.NSerie) Then
                                FileCaricato = False
                                strErrore = "I valori inerenti al 'NSerie' nel documento caricato, non sono indicati su tutte le righe."
                                Exit For
                            End If
                            If DsDocTDL.Lotti.Select("Modello = '" + Controlla_Apice(row.Modello) + "' AND NSerie = '" + Controlla_Apice(row.NSerie) + "'").Length > 1 Then
                                FileCaricato = False
                                strErrore = "I valori inerenti al 'NSerie' nel documento caricato, non sono corretti.</br>E' presente più volte un numero di serie, per lo stesso articolo."
                                Exit For
                            End If
                        Catch ex As Exception
                            FileCaricato = False
                            strErrore = ex.Message.Trim
                            Exit For
                        End Try
                    Next
                    '____________________________________________
                End If

            Else
                FileCaricato = False
                strErrore = "<b>Il file caricato è vuoto.</b>"
            End If
        Else
            If strErrore = "" Then
                strErrore = "Errore imprevisto durante la lettura del file caricato.</br>Verificare che il file caricato sia corretto e abbia tutti i campi compilati.</br>La funzione per il caricamento del file ha dato esito FALSE."
            End If
            Exit Sub
        End If
        If strErrore = "" Then

            Call CaricaDsArticoli()  'giu DsDocTDL alias (ds)

            'Carico dettagli documento
            strSQL = "SELECT * FROM DocumentiD "
            strSQL += "WHERE IDDocumenti=" & DsDocTDL.DocumentiT.Rows(0).Item("IDDocumenti").ToString.Trim
            objDb.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsDocTDL, "DocumentiD")
            'Fine caricamento

            'Testo che gli articoli indicati siano presenti all'interno del documento in esame.
            For Each row As DSDocumenti.ArticoliRow In DsDocTDL.Articoli
                If DsDocTDL.DocumentiD.Select("Cod_Articolo = '" + Controlla_Apice(row.Cod_Articolo) + "'").Length = 0 Then
                    DsDocTDL.Dispose()
                    FileCaricato = False
                    strErrore = "Nel documento caricato è presente almeno un articolo, che non corrisponde agli articoli presenti all'interno del documento indicato."
                    Exit Sub
                End If
            Next
            'Fine test
            Session("dsCsvDettagliDoc") = DsDocTDL
        End If

    End Sub
    Private Sub Inserisci(ByVal Cod_Articolo As String, ByRef strTitolo As String, ByRef strMsg As String)
        Dim rowInsLotto As DataRow
        Dim NColloPartenza As Integer
        Dim Inserito As Boolean = False
        Dim TotQtaArtForRiga As Integer

        Dim dvLotti As New DataView(DsDocTDL.Lotti)
        dvLotti.RowFilter = ("Modello = '" + Controlla_Apice(Cod_Articolo) + "'")

        Dim dvLottiCaricatiInPassato As New DataView(DsLottiUpload.DocumentiDLotti)
        dvLottiCaricatiInPassato.RowFilter = ("Cod_Articolo = '" + Controlla_Apice(Cod_Articolo) + "'")
        Dim TotQtaLottiInPassato As Integer = 0
        For i = 0 To dvLottiCaricatiInPassato.Count - 1
            TotQtaLottiInPassato += dvLottiCaricatiInPassato(i).Item("QtaColli")
        Next

        If dvLotti.Count > 0 Then

            Dim dvRigaArtD As New DataView(DsDocTDL.DocumentiD)
            dvRigaArtD.RowFilter = "(Cod_Articolo = '" + Controlla_Apice(Cod_Articolo) + "')"

            Dim NLottiForArt As Integer = 0
            Dim NLotti As Integer = 0
            Dim TotQtaEvArtSuRighe As Integer = 0
            Try
                If dvRigaArtD.Count > 0 Then

                    TotQtaEvArtSuRighe = 0
                    For i = 0 To dvRigaArtD.Count - 1
                        TotQtaEvArtSuRighe += dvRigaArtD(i).Item("Qta_Evasa")
                    Next

                    If TotQtaEvArtSuRighe < dvLotti.Count + TotQtaLottiInPassato Then
                        lblFile.Text = "Operazione Interrotta!"
                        strTitolo = "Operazione Interrotta!"
                        strMsg = "Nessun valore aggiornato. Si sta cercando di inserire più lotti di quanti sono indicati nel documento riguardo all'articolo:" + "</br>CodArticolo :'" + Cod_Articolo + "'."
                        strMsg = strMsg + "</br>Lotti caricati in precedenza: " + CStr(TotQtaLottiInPassato)
                        strMsg = strMsg + "</br>Lotti che si sta cercando di caricare: " + CStr(dvLotti.Count)
                        strMsg = strMsg + "</br>Lotti richiesti: " + CStr(TotQtaEvArtSuRighe)
                        strMsg = strMsg + "</br>Lotti in eccedenza: " + CStr(dvLotti.Count + TotQtaLottiInPassato - TotQtaEvArtSuRighe)
                        lblFile.Text = strMsg
                        Exit Sub
                    Else
                        For i = 0 To dvRigaArtD.Count - 1
                            'Controllo: esco se ho terminato tutti i lotti da caricare
                            If dvLotti.Count = NLotti Then
                                Exit For
                            End If
                            '---------------------------------------------------------
                            'Verifica che non ci siano altre righe con lo stesso numero di serie
                            '***********************************************************************
                            If DsLottiUpload.DocumentiDLotti.Select("NSerie = '" + dvLotti(NLotti).Item("NSerie").ToString + "' AND Cod_Articolo = '" + CStr(Cod_Articolo) + "'").Length <> 0 Then
                                lblFile.Text = "Nessun valore aggiornato. Il numero di serie che si sta cercando di inserire è già stato inserito in precendenza." + "</br>CodArticolo :'" + Cod_Articolo + "'."
                                strTitolo = "Operazione Interrotta!"
                                strMsg = "Nessun valore aggiornato. Il numero di serie che si sta cercando di inserire è già stato inserito in precendenza." + "</br>CodArticolo :'" + Cod_Articolo + "'."
                                Exit Sub
                            End If
                            '***********************************************************************
                            dvLottiCaricatiInPassato.RowFilter = "Riga = " + CStr(dvRigaArtD(i).Item("Riga")) + ""
                            NColloPartenza = dvLottiCaricatiInPassato.Count
                            TotQtaLottiInPassato = 0
                            For ii = 0 To dvLottiCaricatiInPassato.Count - 1
                                TotQtaLottiInPassato += dvLottiCaricatiInPassato(ii).Item("QtaColli")
                                If NColloPartenza < dvLottiCaricatiInPassato(ii).Item("NCollo") Then
                                    NColloPartenza = dvLottiCaricatiInPassato(ii).Item("NCollo")
                                End If
                            Next
                            TotQtaArtForRiga = dvRigaArtD(i).Item("Qta_Evasa")
                            TotQtaArtForRiga -= TotQtaLottiInPassato
                            If TotQtaArtForRiga < 0 Then
                                lblFile.Text = "Nessun valore aggiornato. Il numero di serie caricati in precendenza sono superiori alla quantità evasa." + "</br>CodArticolo :'" + Cod_Articolo + "'."
                                strTitolo = "Operazione Interrotta!"
                                strMsg = "Nessun valore aggiornato. Il numero di serie caricati in precendenza sono superiori alla quantità evasa." + "</br>CodArticolo :'" + Cod_Articolo + "'."
                                Exit Sub
                            End If
                            If TotQtaArtForRiga > 0 Then
                                NLottiForArt = 0
                                For n = 0 To TotQtaArtForRiga
                                    rowInsLotto = Nothing
                                    rowInsLotto = DsLottiUpload.DocumentiDLottiInsert.NewRow
                                    rowInsLotto.Item("IDDocumenti") = dvRigaArtD(i).Item("IDDocumenti")
                                    rowInsLotto.Item("Riga") = dvRigaArtD(i).Item("Riga")
                                    NColloPartenza = NColloPartenza + 1
                                    rowInsLotto.Item("NCollo") = NColloPartenza
                                    rowInsLotto.Item("Cod_Articolo") = Cod_Articolo
                                    rowInsLotto.Item("QtaColli") = 1
                                    rowInsLotto.Item("Lotto") = dvLotti(NLotti).Item("NLotto") 'giu260124
                                    rowInsLotto.Item("Sfusi") = 0
                                    rowInsLotto.Item("NSerie") = dvLotti(NLotti).Item("NSerie")
                                    DsLottiUpload.DocumentiDLottiInsert.Rows.Add(rowInsLotto)
                                    Inserito = True
                                    NLotti += 1
                                    NLottiForArt += 1
                                    If NLottiForArt = TotQtaArtForRiga Then
                                        Exit For
                                    End If
                                    'Controllo: esco se ho terminato tutti i lotti da caricare
                                    'GIU030320
                                    If dvLotti.Count = NLotti Then
                                        Exit For
                                    End If
                                    '---------
                                Next

                            End If

                        Next

                    End If
                Else
                    dvRigaArtD.Dispose()
                    lblFile.Text = "Errore: Non è stata trovata nessuna corrispondenza inerente all'articolo '" + CStr(Cod_Articolo) + "' per il documento indicato nel file caricato."
                    strTitolo = "Errore!"
                    strMsg = "Errore: Non è stata trovata nessuna corrispondenza inerente all'articolo '" + CStr(Cod_Articolo) + "' per il documento indicato nel file caricato."
                    Exit Sub
                End If
                If Inserito = True Then
                    lblFile.Text = "Valori inseriti/aggiornati correttamente."
                    strRiepFinaleInsert = strRiepFinaleInsert + "Valori aggiornati correttamente." + "</br>CodArticolo :'" + Cod_Articolo + "'.</br>"
                Else
                    lblFile.Text = "Nessun valore inserito/aggiornato."
                    strRiepFinaleInsert = strRiepFinaleInsert + "Nessun valore è stato aggiornato.</br>Sono già stati caricati lotti sufficienti a soddisfare la quantità indicata nel documento." + "</br>CodArticolo :'" + Cod_Articolo + "'.</br>"
                End If
            Catch Ex As Exception
                lblFile.Text = "Errore: " + Ex.Message.Trim
                strTitolo = "Errore! (Sub Inserisci)"
                strMsg = "Errore: " + Ex.Message.Trim
            End Try
        Else
            lblFile.Text = "Non sono stati trovati i Lotti per l'articolo: " & Cod_Articolo.Trim
            strTitolo = "Operazione Interrotta"
            strMsg = "Non sono stati trovati i Lotti. " + "</br>CodArticolo :'" + Cod_Articolo + "'."
        End If
    End Sub
    Private Function RiempiDs() As Boolean 'GIU ByRef ds As DSDocumenti (DsDocTDL)
        Dim riga As String
        Dim row(6) As String 'giu260124
        Dim dsNewRow As DataRow
        strErrore = ""
        DsDocTDL.Lotti.Clear()
        TipoDoc = ""
        Dim sr As New System.IO.StreamReader(Server.MapPath("~/Documenti/Temp/") + CStr(Trim(Session(CSTUTENTE))) + "_Lotti.csv")
        Try
            Do While Not sr.EndOfStream
                riga = ""
                riga = sr.ReadLine

                If Not riga.Trim = "" Then

                    row(0) = ""
                    row(1) = ""
                    row(2) = ""
                    row(3) = ""
                    row(4) = ""
                    row(5) = ""
                    row(6) = ""
                    row = riga.Split(";")
                    If Not row(0).Trim.ToUpper = "ID" Then 'giu130323 trim altrimenti basta uno spazio che carica anche la prima riga e va in errore
                        If TipoDoc.Trim = "" Then
                            sr.Close()
                            sr.Dispose()
                            FileCaricato = False
                            strErrore = "Errore: Non è stato definito il Tipo Documento dopo il N° (DT/FC/...)  </BR>"
                            Return False
                            Exit Function
                        End If
                        dsNewRow = Nothing
                        dsNewRow = DsDocTDL.Lotti.NewRow
                        dsNewRow("ID") = row(0).Trim
                        dsNewRow("CodCliente") = row(1).Trim
                        If LblRiga1.Text.Trim = "" Then LblRiga1.Text = "Codice Cliente: " & row(1).Trim
                        dsNewRow("NumDDT") = row(2).Trim
                        If LblNumDoc.Text.Trim = "" Then LblNumDoc.Text = TipoDoc.Trim + " N° " + row(2).Trim
                        dsNewRow("DataDDT") = row(3).Trim
                        If LblDataDoc.Text.Trim = "" Then LblDataDoc.Text = row(3).Trim
                        dsNewRow("Modello") = row(4).Trim
                        dsNewRow("NSerie") = row(5).Trim
                        dsNewRow("NLotto") = row(6).Trim
                        DsDocTDL.Lotti.Rows.Add(dsNewRow)

                    Else
                        'RK TESTATA VIENE SALTATO E DETERMINO IL TIPO DOC
                        If row(2).Trim.Contains(SWTD(TD.FatturaCommerciale)) And TipoDoc.Trim = "" Then TipoDoc = SWTD(TD.FatturaCommerciale)
                        If row(2).Trim.Contains(SWTD(TD.DocTrasportoClienti)) And TipoDoc.Trim = "" Then TipoDoc = SWTD(TD.DocTrasportoClienti)
                        If row(2).Trim.Contains(SWTD(TD.FatturaAccompagnatoria)) And TipoDoc.Trim = "" Then TipoDoc = SWTD(TD.FatturaAccompagnatoria)
                        If row(2).Trim.Contains(SWTD(TD.CaricoMagazzino)) And TipoDoc.Trim = "" Then TipoDoc = SWTD(TD.CaricoMagazzino)
                        If row(2).Trim.Contains(SWTD(TD.ScaricoMagazzino)) And TipoDoc.Trim = "" Then TipoDoc = SWTD(TD.ScaricoMagazzino)
                        If row(2).Trim.Contains(SWTD(TD.MovimentoMagazzino)) And TipoDoc.Trim = "" Then TipoDoc = SWTD(TD.MovimentoMagazzino)
                        Session.Item("TipoDoc") = TipoDoc
                    End If

                End If
            Loop
            sr.Close()
            sr.Dispose()
            Return True
        Catch ex As Exception
            sr.Close()
            sr.Dispose()
            FileCaricato = False
            strErrore = "Errore: </BR>" + ex.Message
            Return False
            Exit Function
        End Try
        Return True
    End Function
    Private Sub Clear()
        LblDataDoc.Text = ""
        LblNumDoc.Text = ""
        LblRiga1.Text = ""
        LblRiga2.Text = ""
        LblRiga3.Text = ""
        LblRiga4.Text = ""
        LblRiga5.Text = ""
        LblRiga6.Text = ""
        LblRiga7.Text = ""
        LblRiga8.Text = ""
    End Sub
    Private Function SetCdmDAdp() As Boolean

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDocDettL = New SqlConnection
        SqlAdapDocDettL = New SqlDataAdapter
        SqlDbInserCmdL = New SqlCommand

        SqlConnDocDettL.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDbInserCmdL.CommandText = "insert_DocDLByIDDocRiga"
        SqlDbInserCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdL.Connection = Me.SqlConnDocDettL
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

        SqlAdapDocDettL.InsertCommand = SqlDbInserCmdL
    End Function
    Private Function RiempiDatiDoc(ByRef StrErrore As String) As Boolean
        RiempiDatiDoc = False
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            StrErrore = "Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO"
            Exit Function
        End If
        Dim SWSconti As Boolean = False
        Dim TabCliFor As String = ""
        TipoDoc = Session.Item("TipoDoc")
        Try
            DsPrinWebDoc.Clear()
            If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrepEtichette) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                ' ''Session(CSTSWConfermaDoc) = 0
                ' ''Session(CSTDsRitornoEtichette) = "WF_DocumentiElenco.aspx?labelForm=Gestione documenti"
                ' ''Response.Redirect("..\WebFormTables\WF_EtichettePrepara.aspx?labelForm=Prepara etichette")
            Else
                StrErrore = "Errore: " & StrErrore
                Exit Function
            End If
        Catch ex As Exception
            StrErrore = "Errore: " & ex.Message
            Exit Function
        End Try

        'ok riempio
        Dim TmpIntest As New ArrayList
        Dim TmpItem As ListItem
        Dim CAPLocProvCompleto As String = ""
        Dim Tel1 As String = ""
        Try
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
                    If DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1").ToString.Trim <> "" Then
                        TmpItem = New ListItem
                        TmpItem.Text = DsPrinWebDoc.DocumentiT.Rows(0).Item("Destinazione1")
                        TmpIntest.Add(TmpItem)
                    End If
                    If DsPrinWebDoc.DestClienti.Rows.Count > 0 Then
                        If DsPrinWebDoc.DestClienti.Rows(0).Item("Denominazione").ToString.Trim <> "" Then
                            TmpItem = New ListItem
                            TmpItem.Text = DsPrinWebDoc.DestClienti.Rows(0).Item("Denominazione").ToString.Trim
                            TmpIntest.Add(TmpItem)
                        End If
                        If DsPrinWebDoc.DestClienti.Rows(0).Item("Riferimento").ToString.Trim <> "" Then
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
                '-
                LblDataDoc.Text = Format(CDate(DsPrinWebDoc.DocumentiT.Rows(0).Item("Data_Doc").ToString.Trim), "dd/MM/yyyy")
                LblNumDoc.Text = TipoDoc.Trim & "/" & DsPrinWebDoc.DocumentiT.Rows(0).Item("Numero").ToString.Trim

            End If
        Catch ex As Exception
            StrErrore = "Errore: " & ex.Message
            Exit Function
        End Try
        RiempiDatiDoc = True
    End Function

#End Region

End Class