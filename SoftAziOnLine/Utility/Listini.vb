Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Public Class Listini
    Public Shared Sub CambioValuta(ByRef Prezzo_Valuta As Decimal, ByRef Data_Cambio As DateTime, ByRef Cambio As Decimal)
        'DA SVILUPPARE
        Prezzo_Valuta = 0
        Data_Cambio = Nothing
        Cambio = 0
    End Sub

    'Roberto 07/12/2011---
    Public Function StampaListino(ByVal AziendaReport As String, ByVal TitoloReport As String, ByRef dsListino1 As DSListino, ByRef ObjReport As Object, ByRef Errore As String, ByVal myIDListinoT As String, ByVal NomeCampoOrdinamento As String, ByVal CFornitore As String, ByVal DFornitore As String) As Boolean

        Dim Passo As Integer = 0
        If NomeCampoOrdinamento.Trim = "" Then NomeCampoOrdinamento = "Cod_Articolo"
        Dim strErrore As String = ""
        'giu180412 PrezzoAcquisto
        Dim strSQL As String = ""
        strSQL = "SELECT ISNULL(ListVenD.Cod_Articolo, '') AS Cod_Articolo " &
                  ",ISNULL(AnaMag.Descrizione, '') AS Descrizione " &
                  ",ISNULL(Prezzo, 0) AS Prezzo " &
                  ",ISNULL(Prezzo_Valuta, 0) AS Prezzo_Valuta " &
                  ",ISNULL(Sconto_1, 0) AS Sconto_1 " &
                  ",ISNULL(Sconto_2, 0) AS Sconto_2 " &
                  ",ISNULL(PrezzoMinimo, 0) AS Prezzo_Minimo " &
                  ",ISNULL(AnaMag.PrezzoAcquisto, 0) AS PrezzoAcquisto " &
                  ",ISNULL(ListVenT.Codice, '') AS Cod_Listino " &
                  ",ISNULL(ListVenT.Descrizione, '') AS Descrizione_Listino, '" &
                   AziendaReport & "' AS AziendaReport, '" & TitoloReport & "' AS TitoloReport " &
                  ",ISNULL(ScFornitore,0) AS ScontoFornitore " &
                  " FROM ListVenD INNER JOIN " &
                  "      AnaMag ON ListVenD.Cod_Articolo = AnaMag.Cod_Articolo INNER JOIN " &
                  "      ListVenT ON ListVenT.Codice = ListVenD.Codice "
        If myIDListinoT.Trim <> "" Then
            strSQL = strSQL & " WHERE ListVenT.Codice = " & myIDListinoT.Trim
            If CFornitore.Trim <> "" Then
                strSQL += " AND AnaMag.CodiceFornitore= '" & Controlla_Apice(CFornitore.Trim) & "'"
            End If
        Else
            If CFornitore.Trim <> "" Then
                strSQL += " WHERE AnaMag.CodiceFornitore= '" & Controlla_Apice(CFornitore.Trim) & "'"
            End If
        End If


        If NomeCampoOrdinamento.Trim <> "" Then
            strSQL = strSQL & " ORDER BY " & NomeCampoOrdinamento
        End If
        Dim ObjDB As New DataBaseUtility
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsListino1, "ListVenD")
            ObjDB = Nothing
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Listino"
            Return False
            Exit Function
        End Try

        Return True

    End Function
    '------------------------
    Public Function StampaAnaMag(ByVal AziendaReport As String, ByVal TitoloReport As String, ByRef dsAnaMag1 As DSAnaMag, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, Optional ByVal NomeCampoOrdinamento As String = "Cod_Articolo") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strAnaMag As String = "SELECT * FROM AnaMagRiepAnalitico"

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand



        'Riempio AnaMag
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'SqlDbSelectCmd.CommandText = "get_AnaMag"

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        'SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        If NomeCampoOrdinamento.Trim = "Cod_Articolo" Then
            If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
                strAnaMag = strAnaMag & " WHERE Cod_Articolo >= '" & Controlla_Apice(myStrDa.Trim) &
                            "' AND  Cod_Articolo <= '" & Controlla_Apice(myStrA.Trim) & "' "
            End If
        ElseIf NomeCampoOrdinamento.Trim = "Descrizione" Then
            If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
                strAnaMag = strAnaMag & " WHERE Descrizione >= '" & Controlla_Apice(myStrDa.Trim) &
                            "' AND  Descrizione <= '" & Controlla_Apice(myStrA.Trim) & "' "
            End If
        End If
        If NomeCampoOrdinamento.Trim <> "" Then
            strAnaMag = strAnaMag & " ORDER BY " & NomeCampoOrdinamento
        End If

        SqlDbSelectCmd.CommandText = strAnaMag

        'Riempio AliquoteIVA
        Dim SqlAdapAliquoteIva As New SqlDataAdapter
        Dim SqlDbSelectAliquoteIvaCmd As New SqlCommand
        SqlAdapAliquoteIva.SelectCommand = SqlDbSelectAliquoteIvaCmd
        SqlConn2.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDbSelectAliquoteIvaCmd.CommandText = "get_AliquoteIva"
        SqlDbSelectAliquoteIvaCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectAliquoteIvaCmd.Connection = SqlConn2
        SqlDbSelectAliquoteIvaCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        'Riempio Dataset
        SqlAdap.Fill(dsAnaMag1.AnaMag)
        SqlAdapAliquoteIva.Fill(dsAnaMag1.Aliquote_IVA)
        'Ciclo che aÃ¬imposta AziendaReport e Titolo Report per tutte le righe di AliquoteIVA
        Dim r_aliqIva As DSAnaMag.Aliquote_IVARow
        For Each r_aliqIva In dsAnaMag1.Aliquote_IVA
            r_aliqIva.AziendaReport = AziendaReport
            r_aliqIva.TitoloReport = TitoloReport
        Next
        dsAnaMag1.AcceptChanges()


        Return True

    End Function
    'alb200612
    Public Function StampaUbiMag(ByVal AziendaReport As String, ByVal TitoloReport As String, ByRef dsAnaMag1 As DSAnaMag, ByRef ObjReport As Object, ByRef Errore As String, ByVal NomeCampoOrdinamento As String, ByVal CMag As String) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strAnaMag As String = "SELECT ArtdiMag.Codice_Magazzino, AnaMag.Cod_Articolo, AnaMag.Descrizione, ISNULL(ArtdiMag.Reparto, 0) AS Reparto, ISNULL(ArtdiMag.Scaffale, 0) AS Scaffale, ISNULL(ArtdiMag.Piano, 0) AS Piano, ISNULL(ArtDiMag.Sottoscorta,0) AS Sottoscorta "
        strAnaMag += "FROM AnaMag INNER JOIN ArtdiMag ON AnaMag.Cod_Articolo = ArtdiMag.Cod_Articolo WHERE  (ArtdiMag.Codice_Magazzino = " + CMag.Trim + ")"

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand



        'Riempio AnaMag
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'SqlDbSelectCmd.CommandText = "get_AnaMag"

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        'SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        Dim myOrd As String = NomeCampoOrdinamento 'giu230222
        myOrd = myOrd.Replace("Cod_Articolo", "AnaMag.Cod_Articolo")
        myOrd = myOrd.Replace("Descrizione", "AnaMag.Descrizione")
        myOrd = myOrd.Replace("Reparto, Scaffale, Piano, Descrizione", "ArtdiMag.Reparto, ArtdiMag.Scaffale, ArtdiMag.Piano, AnaMag.Descrizione")

        If myOrd.Trim <> "" Then
            strAnaMag = strAnaMag & " ORDER BY " & myOrd
        End If

        SqlDbSelectCmd.CommandText = strAnaMag

        'Riempio Dataset
        SqlAdap.Fill(dsAnaMag1.AnaMag)
        'Ciclo che imposta AziendaReport e Titolo Report per tutte le righe di AliquoteIVA
        Dim r_AnaMag As DSAnaMag.AnaMagRow
        For Each r_AnaMag In dsAnaMag1.AnaMag.Rows
            r_AnaMag.AziendaRpt = AziendaReport
            r_AnaMag.TitoloRpt = TitoloReport
        Next
        dsAnaMag1.AcceptChanges()

        Return True

    End Function
    'giu200717 Richiesta Tel. di Zibordi del 19/07/2017
    Public Function StampaAnaMagFor(ByVal AziendaReport As String, ByVal TitoloReport As String, ByRef dsAnaMag1 As DSAnaMag, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodFornitore As String, Optional ByVal NomeCampoOrdinamento As String = "Cod_Articolo") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strAnaMag As String = "SELECT * FROM AnaMagRiepAnalitico"

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection

        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

        'Riempio AnaMag
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'SqlDbSelectCmd.CommandText = "get_AnaMag"
        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        'SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        If CodFornitore.Trim <> "" Then
            strAnaMag = strAnaMag & " WHERE CodiceFornitore = '" & Controlla_Apice(CodFornitore.Trim) & "'"
        End If
        If NomeCampoOrdinamento.Trim <> "" Then
            strAnaMag = strAnaMag & " ORDER BY " & NomeCampoOrdinamento
        End If
        SqlDbSelectCmd.CommandText = strAnaMag

        'Riempio Prezzo_Ult_Acq
        Dim SqlConn3 As SqlConnection
        SqlConn3 = New SqlConnection
        Dim SqlAdap3 As SqlDataAdapter
        Dim SqlDbSelectCmd3 As SqlCommand
        Dim strPrezzo_Ult_Acq As String = "SELECT Cod_Articolo, MAX(Data_Doc) AS Data_Ult_Acq, Prezzo_Netto AS Prezzo_Ult_Acq " &
                                          "FROM view_MovMagValorizzazione " &
                                          "WHERE (Tipo_Doc = N'CM') " &
                                          "GROUP BY Prezzo_Netto, Cod_Articolo " &
                                          "HAVING      (Prezzo_Netto <> 0) " &
                                          "ORDER BY Cod_Articolo, Data_Ult_Acq DESC "
        SqlAdap3 = New SqlDataAdapter
        SqlDbSelectCmd3 = New SqlCommand
        SqlAdap3.SelectCommand = SqlDbSelectCmd3
        SqlConn3.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'SqlDbSelectCmd.CommandText = "get_AnaMag"
        SqlDbSelectCmd3.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd3.Connection = SqlConn3
        'SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd3.CommandText = strPrezzo_Ult_Acq

        'Rempio Aliquote_IVA anche se in questo caso non mi serve
        Dim SqlAdapAliquoteIva As New SqlDataAdapter
        Dim SqlDbSelectAliquoteIvaCmd As New SqlCommand
        SqlAdapAliquoteIva.SelectCommand = SqlDbSelectAliquoteIvaCmd
        SqlConn2.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDbSelectAliquoteIvaCmd.CommandText = "get_AliquoteIva"
        SqlDbSelectAliquoteIvaCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectAliquoteIvaCmd.Connection = SqlConn2
        SqlDbSelectAliquoteIvaCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        Try
            'Riempio Dataset
            SqlAdap.Fill(dsAnaMag1.AnaMag)
            SqlAdapAliquoteIva.Fill(dsAnaMag1.Aliquote_IVA)
            SqlAdap3.Fill(dsAnaMag1.Prezzo_Ult_Acq)
            'Ciclo che imposta AziendaReport e Titolo Report per tutte le righe di AliquoteIVA
            Dim r_aliqIva As DSAnaMag.Aliquote_IVARow
            For Each r_aliqIva In dsAnaMag1.Aliquote_IVA
                r_aliqIva.AziendaReport = AziendaReport
                r_aliqIva.TitoloReport = TitoloReport
            Next
            'Ciclo che imposta AziendaReport e Titolo Report per tutte le righe di AnaMag 
            'calcola il prezzo PrezzoScontatoFor 
            Dim r_AnaMag As DSAnaMag.AnaMagRow
            Dim myDate As DateTime
            For Each r_AnaMag In dsAnaMag1.AnaMag
                r_AnaMag.AziendaRpt = AziendaReport
                r_AnaMag.TitoloRpt = TitoloReport
                If r_AnaMag.ScFornitore <> 0 And r_AnaMag.PrezzoAcquisto <> 0 Then
                    r_AnaMag.PrezzoScontatoFor = r_AnaMag.PrezzoAcquisto - (r_AnaMag.PrezzoAcquisto * r_AnaMag.ScFornitore / 100)
                Else
                    r_AnaMag.PrezzoScontatoFor = 0
                End If
                r_AnaMag.Prezzo_Ult_Acq = get_UltPrzAcquisto(dsAnaMag1, r_AnaMag.Cod_Articolo, myDate, strErrore)
                If r_AnaMag.Prezzo_Ult_Acq <> 0 Then
                    r_AnaMag.Data_Ult_Acq = myDate
                End If
                If strErrore.Trim <> "" Then
                    Errore = strErrore.Trim
                    Return False
                    Exit Function
                End If
            Next
            dsAnaMag1.AcceptChanges()
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Articoli per Fornitore"
            Return False
            Exit Function
        End Try

        Return True

    End Function
    Private Function get_UltPrzAcquisto(ByVal ds As DSAnaMag, ByVal CodArticolo As String, ByRef myDate As DateTime, ByRef strErrore As String) As Decimal
        Dim view As DataView

        Try
            view = New DataView(ds.Prezzo_Ult_Acq, "Cod_Articolo = '" & CodArticolo & "'", "Data_Ult_Acq DESC", DataViewRowState.CurrentRows)
            If view.Count > 0 Then
                get_UltPrzAcquisto = view.Item(0)("Prezzo_Ult_Acq")
                myDate = view.Item(0)("Data_Ult_Acq")
            Else
                get_UltPrzAcquisto = 0
            End If
        Catch ex As Exception
            strErrore = ex.Message & " - get_UltPrzAcquisto"
            Exit Function
        End Try

    End Function
    'giu010221
    Public Function StampaAnaMagForP(ByVal AziendaReport As String, ByVal TitoloReport As String, ByRef dsAnaMag1 As DSAnaMag, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodFornitore As String, ByVal AllaData As String, Optional ByVal NomeCampoOrdinamento As String = "Cod_Articolo") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strAnaMag As String = "SELECT * FROM AnaMagRiepAnalitico"

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection

        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

        'Riempio AnaMag
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'SqlDbSelectCmd.CommandText = "get_AnaMag"
        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        If CodFornitore.Trim <> "" Then
            strAnaMag = strAnaMag & " WHERE CodiceFornitore = '" & Controlla_Apice(CodFornitore.Trim) & "'"
        End If
        If NomeCampoOrdinamento.Trim <> "" Then
            strAnaMag = strAnaMag & " ORDER BY " & NomeCampoOrdinamento
        End If
        SqlDbSelectCmd.CommandText = strAnaMag

        'Riempio Prezzo_Ult_Acq
        Dim SqlConn3 As SqlConnection
        SqlConn3 = New SqlConnection
        Dim SqlAdap3 As SqlDataAdapter
        Dim SqlDbSelectCmd3 As SqlCommand
        Dim strPrezzo_Ult_Acq As String = ""
        strPrezzo_Ult_Acq += "SELECT Cod_Articolo, MAX(DataRiferimento) AS Data_Ult_Acq, PrezzoAcquisto AS Prezzo_Ult_Acq, ScFornitore " &
                             "FROM AnaMagStorico " '& _
        '' '"WHERE (DataRiferimento <= CONVERT(DATETIME, '" & AllaData.Trim & "', 103)) "
        If CodFornitore.Trim <> "" Then
            '' ' AND
            strPrezzo_Ult_Acq += "WHERE CodiceFornitore ='" & CodFornitore & "' "
        End If
        strPrezzo_Ult_Acq += "GROUP BY Cod_Articolo, PrezzoAcquisto, ScFornitore " &
                             "ORDER BY Cod_Articolo, Data_Ult_Acq DESC "
        SqlAdap3 = New SqlDataAdapter
        SqlDbSelectCmd3 = New SqlCommand
        SqlAdap3.SelectCommand = SqlDbSelectCmd3
        SqlConn3.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDbSelectCmd3.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd3.Connection = SqlConn3
        SqlDbSelectCmd3.CommandText = strPrezzo_Ult_Acq

        'Rempio Aliquote_IVA anche se in questo caso non mi serve
        Dim SqlAdapAliquoteIva As New SqlDataAdapter
        Dim SqlDbSelectAliquoteIvaCmd As New SqlCommand
        SqlAdapAliquoteIva.SelectCommand = SqlDbSelectAliquoteIvaCmd
        SqlConn2.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDbSelectAliquoteIvaCmd.CommandText = "get_AliquoteIva"
        SqlDbSelectAliquoteIvaCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectAliquoteIvaCmd.Connection = SqlConn2
        SqlDbSelectAliquoteIvaCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        Try
            'Riempio Dataset
            SqlAdap.Fill(dsAnaMag1.AnaMag)
            SqlAdapAliquoteIva.Fill(dsAnaMag1.Aliquote_IVA)
            SqlAdap3.Fill(dsAnaMag1.Prezzo_Ult_Acq)
            'Ciclo che imposta AziendaReport e Titolo Report per tutte le righe di AliquoteIVA
            Dim r_aliqIva As DSAnaMag.Aliquote_IVARow
            For Each r_aliqIva In dsAnaMag1.Aliquote_IVA
                r_aliqIva.AziendaReport = AziendaReport
                r_aliqIva.TitoloReport = TitoloReport
            Next
            'Ciclo che imposta AziendaReport e Titolo Report per tutte le righe di AnaMag 
            'calcola il prezzo PrezzoScontatoFor 
            Dim r_AnaMag As DSAnaMag.AnaMagRow
            Dim myDate As DateTime
            Dim myScFornitore As Decimal
            For Each r_AnaMag In dsAnaMag1.AnaMag
                r_AnaMag.AziendaRpt = AziendaReport
                r_AnaMag.TitoloRpt = TitoloReport
                If r_AnaMag.ScFornitore <> 0 And r_AnaMag.PrezzoAcquisto <> 0 Then
                    r_AnaMag.PrezzoScontatoFor = r_AnaMag.PrezzoAcquisto - (r_AnaMag.PrezzoAcquisto * r_AnaMag.ScFornitore / 100)
                Else
                    r_AnaMag.PrezzoScontatoFor = 0
                End If
                r_AnaMag.Prezzo_Ult_Acq = get_UltPrzAcquistoP(dsAnaMag1, r_AnaMag.Cod_Articolo, myDate, myScFornitore, r_AnaMag.PrezzoAcquisto, r_AnaMag.ScFornitore, strErrore)
                If r_AnaMag.Prezzo_Ult_Acq <> 0 Then
                    r_AnaMag.Data_Ult_Acq = myDate
                    r_AnaMag.ScFornitore_Ult_Acq = myScFornitore
                Else
                    r_AnaMag.ScFornitore_Ult_Acq = 0
                End If
                If r_AnaMag.ScFornitore_Ult_Acq <> 0 And r_AnaMag.Prezzo_Ult_Acq <> 0 Then
                    r_AnaMag.PrezzoScontatoForUlt_Acq = r_AnaMag.Prezzo_Ult_Acq - (r_AnaMag.Prezzo_Ult_Acq * r_AnaMag.ScFornitore_Ult_Acq / 100)
                Else
                    r_AnaMag.PrezzoScontatoForUlt_Acq = 0
                End If
                If strErrore.Trim <> "" Then
                    Errore = strErrore.Trim
                    Return False
                    Exit Function
                End If
            Next
            dsAnaMag1.AcceptChanges()
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Articoli per Fornitore - Confronto prezzi"
            Return False
            Exit Function
        End Try

        Return True

    End Function
    Private Function get_UltPrzAcquistoP(ByVal ds As DSAnaMag, ByVal CodArticolo As String, ByRef myDate As DateTime, ByRef myScFornitore As Decimal, ByVal myPrezzoAcq As Decimal, ByVal myScFornAcq As Decimal, ByRef strErrore As String) As Decimal
        Dim view As DataView

        Try
            view = New DataView(ds.Prezzo_Ult_Acq, "Cod_Articolo = '" & CodArticolo & "'", "Data_Ult_Acq DESC", DataViewRowState.CurrentRows)
            If view.Count > 0 Then
                For i = 0 To view.Count - 1
                    get_UltPrzAcquistoP = view.Item(i)("Prezzo_Ult_Acq")
                    myDate = view.Item(i)("Data_Ult_Acq")
                    myScFornitore = view.Item(i)("ScFornitore")
                    If myPrezzoAcq <> view.Item(i)("Prezzo_Ult_Acq") Or myScFornAcq <> view.Item(i)("ScFornitore") Then
                        Exit For
                    End If
                Next
            Else
                get_UltPrzAcquistoP = 0
            End If
        Catch ex As Exception
            strErrore = ex.Message & " - get_UltPrzAcquisto"
            Exit Function
        End Try

    End Function
End Class

Public Class LTDCampiCambio
    Dim MyPrezzo_Valuta As Decimal
    Public Property Prezzo_Valuta() As Decimal
        Get
            Return MyPrezzo_Valuta
        End Get
        Set(ByVal value As Decimal)
            MyPrezzo_Valuta = value
        End Set
    End Property
    Dim MyData_Cambio As DateTime
    Public Property Data_Cambio() As DateTime
        Get
            Return MyData_Cambio
        End Get
        Set(ByVal value As DateTime)
            MyData_Cambio = value
        End Set
    End Property
    Dim MyCambio As Decimal
    Public Property Cambio() As Decimal
        Get
            Return MyCambio
        End Get
        Set(ByVal value As Decimal)
            MyCambio = value
        End Set
    End Property
End Class


