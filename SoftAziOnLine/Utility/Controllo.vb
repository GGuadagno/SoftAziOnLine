Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework

Public Class Controllo

    Public Function StampaDiffPrezzoListino(ByVal Tipo_Doc As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal _Esercizio As String, ByRef DSDiffPrezzoListino1 As DSControlli, ByRef Errore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapDiffPrezzoListino As SqlDataAdapter
        Dim SqlDbSelectDiffPrezzoListino As SqlCommand
        Try
            DSDiffPrezzoListino1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapDiffPrezzoListino = New SqlDataAdapter
            SqlDbSelectDiffPrezzoListino = New SqlCommand

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
            SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
            '---------------------------
            SqlAdapDiffPrezzoListino.SelectCommand = SqlDbSelectDiffPrezzoListino
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectDiffPrezzoListino.CommandType = System.Data.CommandType.StoredProcedure

            SqlDbSelectDiffPrezzoListino.CommandText = "get_StatDiffPrezzoListino"

            SqlDbSelectDiffPrezzoListino.Connection = SqlConnOrd

            SqlDbSelectDiffPrezzoListino.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDiffPrezzoListino.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoDoc", System.Data.SqlDbType.NVarChar, 2, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDiffPrezzoListino.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDiffPrezzoListino.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            If Tipo_Doc.Trim.Length > 0 Then
                SqlDbSelectDiffPrezzoListino.Parameters.Item("@TipoDoc").Value = Tipo_Doc.Trim
            End If
            If txtDataDa.Trim.Length > 0 Then
                SqlDbSelectDiffPrezzoListino.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa.Trim), FormatoData)
            End If
            If txtDataA.Trim.Length > 0 Then
                SqlDbSelectDiffPrezzoListino.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA.Trim), FormatoData)
            End If

            SqlAdapDiffPrezzoListino.Fill(DSDiffPrezzoListino1.DiffPrezzoListino)

            If DSDiffPrezzoListino1.DiffPrezzoListino.Count > 0 Then
                For Each row As DSControlli.DiffPrezzoListinoRow In DSDiffPrezzoListino1.DiffPrezzoListino
                    row.AziendaRpt = Esercizio
                    row.TitoloRpt = "Controllo differenza prezzo / prezzo di listino"
                    row.PiedeRpt = ""
                    If Tipo_Doc.Trim.Length > 0 Then
                        row.PiedeRpt = row.PiedeRpt & "Tipo documento: " & Tipo_Doc.Trim & ". "
                    End If
                    If txtDataDa.Trim.Length > 0 Then
                        row.PiedeRpt = row.PiedeRpt & "Dalla data: " & txtDataDa.Trim
                    End If
                    If txtDataA.Trim.Length > 0 Then
                        row.PiedeRpt = row.PiedeRpt & " Alla data: " & txtDataA.Trim
                    End If
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa di controllo prezzo / prezzo di listino"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    Public Function StampaDiffImportoRiga(ByVal Tipo_Doc As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal _Esercizio As String, ByRef DSDiffImportoRiga1 As DSControlli, ByRef Errore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapDiffImportoRiga As SqlDataAdapter
        Dim SqlDbSelectDiffImportoRiga As SqlCommand
        Try
            DSDiffImportoRiga1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapDiffImportoRiga = New SqlDataAdapter
            SqlDbSelectDiffImportoRiga = New SqlCommand

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
            SqlDbSelectDiffImportoRiga.CommandTimeout = myTimeOUT

            SqlAdapDiffImportoRiga.SelectCommand = SqlDbSelectDiffImportoRiga
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectDiffImportoRiga.CommandType = System.Data.CommandType.StoredProcedure

            SqlDbSelectDiffImportoRiga.CommandText = "get_DiffImportoRiga"

            SqlDbSelectDiffImportoRiga.Connection = SqlConnOrd

            SqlDbSelectDiffImportoRiga.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDiffImportoRiga.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoDoc", System.Data.SqlDbType.NVarChar, 2, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDiffImportoRiga.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDiffImportoRiga.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            If Tipo_Doc.Trim.Length > 0 Then
                SqlDbSelectDiffImportoRiga.Parameters.Item("@TipoDoc").Value = Tipo_Doc.Trim
            End If
            If txtDataDa.Trim.Length > 0 Then
                SqlDbSelectDiffImportoRiga.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa.Trim), FormatoData)
            End If
            If txtDataA.Trim.Length > 0 Then
                SqlDbSelectDiffImportoRiga.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA.Trim), FormatoData)
            End If

            SqlAdapDiffImportoRiga.Fill(DSDiffImportoRiga1.DiffImportoRiga)

            If DSDiffImportoRiga1.DiffImportoRiga.Count > 0 Then
                For Each row As DSControlli.DiffImportoRigaRow In DSDiffImportoRiga1.DiffImportoRiga
                    row.AziendaRpt = Esercizio
                    row.TitoloRpt = "Controllo differenza importo riga diverso dall'importo ricalcolato"
                    row.PiedeRpt = ""
                    If Tipo_Doc.Trim.Length > 0 Then
                        row.PiedeRpt = row.PiedeRpt & "Tipo documento: " & Tipo_Doc.Trim & ". "
                    End If
                    If txtDataDa.Trim.Length > 0 Then
                        row.PiedeRpt = row.PiedeRpt & "Dalla data: " & txtDataDa.Trim
                    End If
                    If txtDataA.Trim.Length > 0 Then
                        row.PiedeRpt = row.PiedeRpt & " Alla data: " & txtDataA.Trim
                    End If
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa di controllo importo riga"
            Return False
            Exit Function
        End Try

        Return True

    End Function
    'GIU230822 GIU200323 Ric. Erika Blocco emissione DDT/Fatture se manca di P.IVA/C.F.
    'giu180423 blocco se manca dest.merci o dati corriere 
    Public Function CKCliNoFattByIDDoc(ByVal myID As String, ByRef NoPIVACF As Boolean, ByRef NoCodIPA As Boolean, ByRef NODestM As Boolean, ByRef NODatiCorr As Boolean, ByRef NoCVett As Boolean, ByRef strMess As String) As Boolean
        strMess = ""
        CKCliNoFattByIDDoc = False
        NoPIVACF = False
        NoCodIPA = False
        NODatiCorr = False
        NODestM = False
        NoCVett = False
        If Not IsNumeric(myID.Trim) Then
            strMess = "Attenzione,IDENTIFICATIVO Documento ERRATO."
            Exit Function
        End If

        Dim strSQL As String = "SELECT   ISNULL(Clienti.NoFatt, 0) AS NoFatt, ISNULL(Clienti.Codice_Fiscale, '') AS CF, ISNULL(Clienti.Partita_IVA, '') AS PIVA, " & _
                " ISNULL(Clienti.IPA, N'') AS CodIPA, ISNULL(DocumentiT.Cod_Filiale,0) AS Cod_Filiale, ISNULL(DestClienti.Progressivo,0) AS Progressivo, " & _
                " ISNULL(DestClienti.Ragione_Sociale35,'') AS RagSoc35, ISNULL(DestClienti.Riferimento35,'') AS Rif35, ISNULL(DocumentiT.Vettore_1,0) AS CVett " & _
                " FROM Clienti INNER JOIN DocumentiT ON Clienti.Codice_CoGe = DocumentiT.Cod_Cliente LEFT OUTER JOIN " & _
                " DestClienti ON DocumentiT.Cod_Cliente = DestClienti.Codice AND DocumentiT.Cod_Filiale = DestClienti.Progressivo " & _
                " WHERE (DocumentiT.IDDocumenti = " & myID.Trim & ")"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    If row(0).Item("NoFatt") <> 0 Then
                        CKCliNoFattByIDDoc = True
                    End If
                    'GIU200323
                    If row(0).Item("CF").ToString.Trim = "" And row(0).Item("PIVA").ToString.Trim = "" Then
                        NoPIVACF = True
                    End If
                    'giu170423 email ERIKA/FRANCESCA 
                    If row(0).Item("CodIPA").ToString.Trim = "" Then
                        NoCodIPA = True
                    End If
                    If row(0).Item("Cod_Filiale").ToString.Trim = "" Or row(0).Item("Cod_Filiale").ToString.Trim = "0" Or _
                       row(0).Item("Progressivo").ToString.Trim = "" Or row(0).Item("Progressivo").ToString.Trim = "0" Then
                        NODestM = True
                    ElseIf row(0).Item("RagSoc35").ToString.Trim = "" Or row(0).Item("Rif35").ToString.Trim = "" Then
                        NODatiCorr = True
                    End If
                    If row(0).Item("CVett") <> 0 Then
                        NoCVett = False
                    Else
                        NoCVett = True
                    End If
                Else
                    strMess = "Non Trovato DocumentiT/Clienti/DestMerce"
                    Exit Function
                End If
            Else
                strMess = "Non Trovato DocumentiT/Clienti/DestMerce"
                Exit Function
            End If
        Catch Ex As Exception
            CKCliNoFattByIDDoc = False
            NoPIVACF = False
            NoCodIPA = False
            NODatiCorr = False
            NODestM = False
            NoCVett = False
            strMess = "Errore: Lettura DocumentiT/Clienti/DestMerce - " & Ex.Message
            Exit Function
        End Try
    End Function
    'giu190723
    'giu180723 per i caratteri speciali NSerie/Lotto
    Private Function NoCarSpecNoteSL(ByVal pNoteRitiro As String, ByRef SWCKSerieLotto As Boolean) As String
        NoCarSpecNoteSL = ""
        SWCKSerieLotto = False
        '------------------------------------------
        Dim myPos As Integer = 0
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim ListaSL As New List(Of String)
        Dim StrDato() As String
        Dim SAVEDato As String = ""
        myPos = InStr(pNoteRitiro, "§")
        If myPos > 0 Then
            StrDato = pNoteRitiro.Trim.Split("§")
            For I = 0 To StrDato.Count - 1
                SAVEDato = StrDato(I)
                mySL = Formatta.FormattaNomeFile(StrDato(I))
                If SAVEDato <> mySL Then
                    SWCKSerieLotto = True
                End If
                If I > StrDato.Count - 1 Then
                    myNoteRitiro = ""
                Else
                    I += 1
                    myNoteRitiro = StrDato(I)
                End If
                ListaSL.Add(mySL + "§" + myNoteRitiro.Trim)
            Next
            'ok qui ripasso tutta la stringa senza i caratteri
            For II = 0 To ListaSL.Count - 1
                If NoCarSpecNoteSL.Trim <> "" Then
                    NoCarSpecNoteSL += "§"
                End If
                NoCarSpecNoteSL += ListaSL.Item(II).Trim
            Next
            If NoCarSpecNoteSL.Trim = "" Then
                NoCarSpecNoteSL = pNoteRitiro.Trim
            End If
        Else 'c'è una descrizione ma non assegnata a nessuna quindi appartiene a tutti i N° di serie
            'vediamo dopo Call SetNoteSLALLApp(pNoteRitiro)
            NoCarSpecNoteSL = pNoteRitiro.Trim
        End If
    End Function

    Dim myIDDoc As String = "" : Dim myNDoc As String = "" : Dim myDataDoc As String = "" : Dim myTipoDoc As String = ""
    Dim myDurataNum As String = "" : Dim myDurataNumRiga As String = "" : Dim myRiga As String = ""
    Public Function ContrattiCKSerieLotto(ByRef DsPrinWebDoc As DSPrintWeb_Documenti, ByVal SWAggiorna As Boolean, ByRef Errore As String) As Boolean
        ContrattiCKSerieLotto = False
        '
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim Passo As Integer = 0
        Dim SqlConnDoc As SqlConnection
        Dim SqlAdapDoc As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        SqlConnDoc = New SqlConnection
        SqlAdapDoc = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

        SqlAdapDoc.SelectCommand = SqlDbSelectCmd
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDbSelectCmd.CommandText = "get_ConTByIDDocumenti"
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = SqlConnDoc
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        Dim SqlAdapDocDett As New SqlDataAdapter
        Dim SqlDbSelectDettCmd As New SqlCommand
        SqlDbSelectDettCmd.CommandText = "get_ConDByIDDocumenti"
        SqlDbSelectDettCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectDettCmd.Connection = SqlConnDoc
        SqlDbSelectDettCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        SqlAdapDocDett.SelectCommand = SqlDbSelectDettCmd
        '-
        Try
            '==============CARICAMENTO DATASET ===================
            Passo = 1
            DsPrinWebDoc.Clear()
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = DBNull.Value 'IdDocumento
            SqlAdapDoc.Fill(DsPrinWebDoc.ContrattiT)
        Catch ex As Exception
            Errore = ex.Message & " - Lettura testate Contratto Passo: " & Passo.ToString & " <br> "
            Return False
            Exit Function
        End Try
        '---------------------------------------------------------------------------------------------
        Dim SWCKSerieLotto As Boolean = False
        Dim strSaveDato As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim StrSql As String = ""
        Dim SWOk As Boolean = True
        '--------------------------------------------------------------------------------------------------
        Dim rsTestata As DataRow
        Dim rsDettagli As DataRow
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                For Each rsTestata In DsPrinWebDoc.Tables("ContrattiT").Select("")
                    Passo = 2
                    myIDDoc = rsTestata!IDDocumenti.ToString.Trim
                    myDataDoc = IIf(IsDBNull(rsTestata!Data_Doc), "", rsTestata!Data_Doc)
                    myNDoc = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero)
                    myTipoDoc = IIf(IsDBNull(rsTestata!Tipo_Doc), "", rsTestata!Tipo_Doc)
                    '
                    rsTestata.BeginEdit()
                    If IsDBNull(rsTestata!Cod_Filiale) Then rsTestata!Cod_Filiale = 0
                    If IsDBNull(rsTestata!Destinazione1) Then rsTestata!Destinazione1 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione2 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione3 = ""
                    rsTestata!Iniziali = ""
                    '---------
                    If IsDBNull(rsTestata!NoteRitiro) Then rsTestata!NoteRitiro = ""
                    strSaveDato = rsTestata!NoteRitiro.ToString.Trim
                    '---------
                    rsTestata!NoteRitiro = NoCarSpecNoteSL(rsTestata!NoteRitiro.ToString.Trim, SWCKSerieLotto)
                    If SWCKSerieLotto And SWAggiorna Then
                        rsTestata!Iniziali = rsTestata!NoteRitiro.ToString.Trim
                        Try
                            'giu040324 corretto errore ''''''' aggiungeva ad ogni agg un apice in piu tolto il contolla_apice
                            SWOk = ObjDB.ExecUpgNoteRitiro(myIDDoc.Trim, rsTestata!NoteRitiro.ToString.Trim, Errore)
                            If SWOk = False Then
                                ObjDB = Nothing
                                Errore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgNoteIntervento)"
                                Return False
                                Exit Function
                            End If
                            ObjDB = Nothing
                        Catch Ex As Exception
                            Errore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgNoteIntervento) - " & Ex.Message.Trim & " -  " & Errore
                            Return False
                            Exit Function
                        End Try
                    ElseIf SWCKSerieLotto Then
                        rsTestata!Iniziali = rsTestata!NoteRitiro.ToString.Trim
                    End If
                    '-Dettagli
                    If SWAggiorna Then
                        DsPrinWebDoc.ContrattiD.Clear()
                        SqlDbSelectDettCmd.Parameters.Item("@IDDocumenti").Value = myIDDoc
                        SqlDbSelectDettCmd.Parameters.Item("@DurataNum").Value = DBNull.Value '1 'fisso per le attività per periodo
                        SqlDbSelectDettCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
                        SqlAdapDocDett.Fill(DsPrinWebDoc.ContrattiD)
                        '-
                        Passo = 3

                        If (DsPrinWebDoc.Tables("ContrattiD").Rows.Count > 0) Then
                            For Each rsDettagli In DsPrinWebDoc.Tables("ContrattiD").Select("", "Riga")
                                SWCKSerieLotto = False
                                myDurataNum = IIf(IsDBNull(rsDettagli!DurataNum), "", rsDettagli!DurataNum)
                                myDurataNumRiga = IIf(IsDBNull(rsDettagli!DurataNumRiga), "", rsDettagli!DurataNumRiga)
                                myRiga = IIf(IsDBNull(rsDettagli!Riga), "", rsDettagli!Riga)
                                '-
                                rsDettagli.BeginEdit()
                                If IsDBNull(rsDettagli!Serie) Then rsDettagli!Serie = ""
                                strSaveDato = rsDettagli!Serie.ToString.Trim
                                rsDettagli!Serie = Formatta.FormattaNomeFile(rsDettagli!Serie)
                                If strSaveDato <> rsDettagli!Serie.ToString.Trim Then
                                    SWCKSerieLotto = True
                                    If rsTestata!Iniziali.ToString.Trim = "" Then
                                        rsTestata!Iniziali = rsDettagli!Serie
                                    End If
                                End If
                                '-
                                If IsDBNull(rsDettagli!Lotto) Then rsDettagli!Lotto = ""
                                strSaveDato = rsDettagli!Lotto.ToString.Trim
                                rsDettagli!Lotto = Formatta.FormattaNomeFile(rsDettagli!Lotto)
                                If strSaveDato <> rsDettagli!Lotto.ToString.Trim Then
                                    SWCKSerieLotto = True
                                    If rsTestata!Iniziali.ToString.Trim = "" Then
                                        rsTestata!Iniziali = rsDettagli!Lotto
                                    End If
                                End If
                                '-
                                rsDettagli.EndEdit()
                                rsDettagli.AcceptChanges()
                                '-
                                If SWCKSerieLotto = True And SWAggiorna Then
                                    Try
                                        StrSql = "Update ContrattiD Set Lotto = '" & rsDettagli!Lotto.ToString.Trim & "', Serie = '" & rsDettagli!Serie.ToString.Trim & "'" & _
                                        " Where IdDocumenti = " & myIDDoc.Trim & " And DurataNum = " & myDurataNum.Trim & " And DurataNumRiga = " & myDurataNumRiga & _
                                        " And Riga = " & myRiga
                                        If ObjDB Is Nothing Then
                                            ObjDB = New DataBaseUtility
                                        End If
                                        ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, StrSql)
                                    Catch ex As Exception
                                        Errore = ex.Message & " - Errore - Aggiorna Dettagli. Passo: " & Passo.ToString & " <br> " & _
                                        "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                                        Return False
                                        Exit Function
                                    End Try
                                End If
                            Next
                        End If
                    End If
                    '-
                    rsTestata.EndEdit()
                    rsTestata.AcceptChanges()
                    '-
                Next
            Else
                Errore = "Errore - Contratto lettura testata+CaricoDettagli. Passo: " & Passo.ToString & " <br> " & _
                "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Errore - Contratto lettura testata+CaricoDettagli. Passo: " & Passo.ToString & " <br> " & _
            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '==FINE CARICAMENTO ====================================
        ObjDB = Nothing

        Return True

    End Function

    'giu181223 Aggiunta nuovo cammpi Elenco Scadenze: TotFatturato (TotResiduo è la differenza tra Importo e TotFatturato)
    Public Function AggContrattiScadPagCA(ByRef DsPrinWebDoc As DSPrintWeb_Documenti, ByRef Errore As String) As Boolean
        AggContrattiScadPagCA = False
        '
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim Passo As Integer = 0
        Dim SqlConnDoc As SqlConnection
        Dim SqlAdapDoc As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        SqlConnDoc = New SqlConnection
        SqlAdapDoc = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

        SqlAdapDoc.SelectCommand = SqlDbSelectCmd
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDbSelectCmd.CommandText = "get_ConTByIDDocumenti"
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = SqlConnDoc
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '-
        Try
            '==============CARICAMENTO DATASET ===================
            Passo = 1
            DsPrinWebDoc.Clear()
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = DBNull.Value 'IdDocumento
            SqlAdapDoc.Fill(DsPrinWebDoc.ContrattiT)
        Catch ex As Exception
            Errore = ex.Message & " - Lettura testate Contratto Passo: " & Passo.ToString & " <br> "
            Return False
            Exit Function
        End Try
        '---------------------------------------------------------------------------------------------
        Dim ObjDB As New DataBaseUtility
        Dim StrSql As String = ""
        Dim SWOk As Boolean = True
        Dim rowScadPagCa As ScadPagCAEntity = Nothing
        Dim lineaSplit As String() = Nothing
        Dim myScadCA As ScadPagCAEntity = Nothing
        Dim myScadPagCA As String = ""
        Dim NRate As Integer = 0
        Dim TotRate As Decimal = 0
        Dim I As Integer = 0
        Dim c
        Dim ArrScadPagCA As ArrayList = Nothing
        Dim strScadPagCA As String = ""
        '--------------------------------------------------------------------------------------------------
        Dim rsTestata As DataRow
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                Passo = 2
                For Each rsTestata In DsPrinWebDoc.Tables("ContrattiT").Select("")
                    '-
                    myIDDoc = rsTestata!IDDocumenti.ToString.Trim
                    myDataDoc = IIf(IsDBNull(rsTestata!Data_Doc), "", rsTestata!Data_Doc)
                    myNDoc = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero)
                    myTipoDoc = IIf(IsDBNull(rsTestata!Tipo_Doc), "", rsTestata!Tipo_Doc)
                    '
                    'giu240420 RATE FATTURAZIONE 
                    strScadPagCA = ""
                    If Not IsDBNull(rsTestata!ScadPagCA) Then
                        strScadPagCA = rsTestata!ScadPagCA.ToString.Trim
                        If String.IsNullOrEmpty(strScadPagCA.Trim) Then
                            strScadPagCA = ""
                        End If
                    End If
                    '-
                    If strScadPagCA.Trim <> "" Then
                        Try
                            lineaSplit = strScadPagCA.Trim.Split(";")
                            'controllo che non già stato fatto /6 senza decimali
                            '''TotRate = 0
                            '''For I = 1 To Len(strScadPagCA)
                            '''    c = Mid(strScadPagCA, I, 1)
                            '''    If c = ";" Then
                            '''        TotRate += 1
                            '''    End If
                            '''Next
                            '''NRate = TotRate / 6
                            '''TotRate = (lineaSplit.Count) / 6
                            '''If NRate <> TotRate Then
                            '''    '''Errore = "ATTENZIONE, Aggiornamento già effettuato. (nnn/6)If NRate - TotRate <> 0 Then) Passo: " & Passo.ToString & ""
                            '''    '''Return False
                            '''    '''Exit Function
                            '''    Continue For
                            '''End If
                            '---------------------------------------------------
                            ArrScadPagCA = New ArrayList
                            If strScadPagCA.Trim <> "" Then
                                For I = 0 To lineaSplit.Count - 1
                                    If lineaSplit(I).Trim <> "" And (I + 6) <= lineaSplit.Count - 1 Then

                                        myScadCA = New ScadPagCAEntity
                                        myScadCA.NRata = lineaSplit(I).Trim
                                        I += 1
                                        myScadCA.Data = lineaSplit(I).Trim
                                        I += 1
                                        myScadCA.Importo = lineaSplit(I).Trim
                                        TotRate += CDec(myScadCA.Importo)
                                        I += 1
                                        myScadCA.Evasa = lineaSplit(I).Trim
                                        I += 1
                                        myScadCA.NFC = lineaSplit(I).Trim
                                        I += 1
                                        myScadCA.DataFC = lineaSplit(I).Trim
                                        I += 1
                                        myScadCA.Serie = lineaSplit(I).Trim
                                        myScadCA.ImportoF = "0"
                                        myScadCA.ImportoR = myScadCA.Importo
                                        ArrScadPagCA.Add(myScadCA)
                                        NRate += 1
                                    End If
                                Next
                            End If
                            '-
                        Catch ex As Exception
                            Errore = ex.Message & " - Tab. ScadPagCA" & " <br> " & _
                                "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End Try
                    End If
                    '-
                    '---------
                    myScadPagCA = ""
                    For I = 0 To ArrScadPagCA.Count - 1
                        If myScadPagCA.Trim <> "" Then myScadPagCA += ";"
                        rowScadPagCa = ArrScadPagCA(I)
                        myScadPagCA += rowScadPagCa.NRata.Trim & ";"
                        myScadPagCA += rowScadPagCa.Data.Trim & ";"
                        myScadPagCA += rowScadPagCa.Importo.Trim & ";"
                        myScadPagCA += rowScadPagCa.Evasa.ToString.Trim & ";"
                        myScadPagCA += rowScadPagCa.NFC.Trim & ";"
                        myScadPagCA += rowScadPagCa.DataFC.Trim & ";"
                        myScadPagCA += rowScadPagCa.Serie.Trim & ";"
                        myScadPagCA += rowScadPagCa.ImportoF & ";" 'giu191223
                        myScadPagCA += rowScadPagCa.ImportoR
                    Next
                    'OK AGGIORNO
                    Try
                        SWOk = ObjDB.ExecUpgScadPagCA(myIDDoc.Trim, myScadPagCA.ToString.Trim, Errore)
                        If SWOk = False Then
                            ObjDB = Nothing
                            Errore = "Errore: Si è verificato un errore durante l'aggiornamento testata (ExecUpgScadPagCA)"
                            Return False
                            Exit Function
                        End If
                    Catch Ex As Exception
                        Errore = "Errore: Si è verificato un errore durante l'aggiornamento testata (ExecUpgScadPagCA) - " & Ex.Message.Trim & " -  " & Errore
                        Return False
                        Exit Function
                    End Try
                Next
            Else
                Errore = "ATTENZIONE, Nessun contratto presente. Passo: " & Passo.ToString & ""
                Return False
                Exit Function
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Errore - Contratto lettura Testata. Passo: " & Passo.ToString & " <br> " & _
            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '==FINE CARICAMENTO ====================================
        ObjDB = Nothing

        Return True

    End Function
End Class