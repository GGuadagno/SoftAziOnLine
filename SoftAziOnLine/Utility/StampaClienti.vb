Imports SoftAziOnLine.Def
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Public Class StampaClienti
    Public Function StampaClienti(ByVal AzReport As String, ByVal dsClientiStampa As dsClienti, ByRef Errore As String, ByVal TipoStampa As Integer, ByVal Ordinamento As String, ByVal Where As String, ByVal myStrFiltri As String, Optional ByVal dettagli As Integer = -1) As Boolean
        StampaClienti = True
        Dim DBConn As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnCli As SqlConnection
        Dim SqlAdapCli As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strORD As String = ""

        Dim strValore As String = "0"
        Dim strErrore As String = ""
        'giu190617
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
        '---------------------------
        Try
            dsClientiStampa.Clear()
            dsClientiStampa.EnforceConstraints = False 'giu030223 per evitare errori su campi null
            If Ordinamento <> "" Then
                strORD = " ORDER BY " & Ordinamento
            End If
            If TipoStampa = 1 Then 'elenco sintetico 
                SqlConnCli = New SqlConnection(DBConn.getConnectionString(TipoDB.dbSoftCoge))

                SqlDbSelectCmd = New SqlCommand()
                SqlDbSelectCmd.CommandTimeout = myTimeOUT
                SqlAdapCli = New SqlDataAdapter()

                SqlAdapCli.SelectCommand = SqlDbSelectCmd
                SqlDbSelectCmd.CommandText = "Select * From Clienti " & Where & strORD
                SqlDbSelectCmd.CommandType = CommandType.Text
                SqlDbSelectCmd.Connection = SqlConnCli
                SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

                SqlAdapCli.Fill(dsClientiStampa.vi_CliSint)
                For Each row As dsClienti.vi_CliSintRow In dsClientiStampa.vi_CliSint
                    row.BeginEdit()
                    row.Ditta = AzReport
                    row.Filtri = myStrFiltri
                    row.EndEdit()
                Next
                dsClientiStampa.AcceptChanges()
            ElseIf TipoStampa = 2 Then 'elenco Rubrica 
                SqlConnCli = New SqlConnection(DBConn.getConnectionString(TipoDB.dbSoftCoge))

                SqlDbSelectCmd = New SqlCommand()
                SqlAdapCli = New SqlDataAdapter()
                SqlDbSelectCmd.CommandTimeout = myTimeOUT

                Select Case dettagli
                    Case -1
                        SqlAdapCli.SelectCommand = SqlDbSelectCmd
                        SqlDbSelectCmd.CommandText = "Select * from Clienti " & Where & strORD
                        SqlDbSelectCmd.CommandType = CommandType.Text
                        SqlDbSelectCmd.Connection = SqlConnCli
                        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                        SqlAdapCli.Fill(dsClientiStampa.vi_CliSint)
                        For Each row As dsClienti.vi_CliSintRow In dsClientiStampa.vi_CliSint
                            row.BeginEdit()
                            row.Ditta = AzReport
                            row.Filtri = myStrFiltri
                            row.EndEdit()
                        Next
                        dsClientiStampa.AcceptChanges()
                    Case 0
                        SqlAdapCli.SelectCommand = SqlDbSelectCmd
                        SqlDbSelectCmd.CommandText = "Select * from Clienti " & Where & strORD
                        SqlDbSelectCmd.CommandType = CommandType.Text
                        SqlDbSelectCmd.Connection = SqlConnCli
                        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                        SqlAdapCli.Fill(dsClientiStampa.vi_CliSint)
                        For Each row As dsClienti.vi_CliSintRow In dsClientiStampa.vi_CliSint
                            row.BeginEdit()
                            row.Ditta = AzReport
                            row.Filtri = myStrFiltri
                            row.EndEdit()
                        Next
                        dsClientiStampa.AcceptChanges()
                    Case 1
                        SqlAdapCli.SelectCommand = SqlDbSelectCmd
                        SqlDbSelectCmd.CommandText = "Select * from Clienti inner join IndirCF on Clienti.Codice_CoGe = IndirCF.Codice " & Where & strORD
                        SqlDbSelectCmd.CommandType = CommandType.Text
                        SqlDbSelectCmd.Connection = SqlConnCli
                        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                        SqlAdapCli.Fill(dsClientiStampa.vi_CliSint)
                        For Each row As dsClienti.vi_CliSintRow In dsClientiStampa.vi_CliSint
                            row.BeginEdit()
                            row.Ditta = AzReport
                            row.Filtri = myStrFiltri
                            row.EndEdit()
                        Next
                        dsClientiStampa.AcceptChanges()

                        SqlDbSelectCmd = Nothing

                        SqlDbSelectCmd = New SqlCommand("Select * from IndirCF", SqlConnCli)
                        SqlAdapCli.SelectCommand = SqlDbSelectCmd

                        SqlAdapCli.Fill(dsClientiStampa.IndirCF)
                    Case 2
                        SqlAdapCli.SelectCommand = SqlDbSelectCmd
                        SqlDbSelectCmd.CommandText = "Select * from Clienti inner join DestClienti on Clienti.Codice_CoGe = DestClienti.Codice " & Where & strORD
                        SqlDbSelectCmd.CommandType = CommandType.Text
                        SqlDbSelectCmd.Connection = SqlConnCli
                        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                        SqlAdapCli.Fill(dsClientiStampa.vi_CliSint)
                        For Each row As dsClienti.vi_CliSintRow In dsClientiStampa.vi_CliSint
                            row.BeginEdit()
                            row.Ditta = AzReport
                            row.Filtri = myStrFiltri
                            row.EndEdit()
                        Next
                        dsClientiStampa.AcceptChanges()

                        SqlDbSelectCmd = Nothing

                        SqlDbSelectCmd = New SqlCommand("Select * from DestClienti", SqlConnCli)
                        SqlAdapCli.SelectCommand = SqlDbSelectCmd

                        SqlAdapCli.Fill(dsClientiStampa.DestClienti)
                End Select
            ElseIf TipoStampa = 3 Then 'elenco analitico
                SqlConnCli = New SqlConnection(DBConn.getConnectionString(TipoDB.dbSoftCoge))

                SqlDbSelectCmd = New SqlCommand()
                SqlAdapCli = New SqlDataAdapter()
                SqlDbSelectCmd.CommandTimeout = myTimeOUT

                SqlAdapCli.SelectCommand = SqlDbSelectCmd
                SqlDbSelectCmd.CommandText = "Select * From vi_CliAnalit " & Where & strORD
                SqlDbSelectCmd.CommandType = CommandType.Text
                SqlDbSelectCmd.Connection = SqlConnCli

                SqlAdapCli.Fill(dsClientiStampa.vi_CliAnalit)

                For Each row As dsClienti.vi_CliAnalitRow In dsClientiStampa.vi_CliAnalit
                    row.BeginEdit()
                    row.Ditta = AzReport
                    row.Filtri = myStrFiltri
                    row.EndEdit()
                Next

                dsClientiStampa.AcceptChanges()
            ElseIf TipoStampa = 4 Then 'elenco codici
                SqlConnCli = New SqlConnection(DBConn.getConnectionString(TipoDB.dbSoftCoge))

                SqlDbSelectCmd = New SqlCommand()
                SqlAdapCli = New SqlDataAdapter()
                SqlDbSelectCmd.CommandTimeout = myTimeOUT

                SqlAdapCli.SelectCommand = SqlDbSelectCmd
                SqlDbSelectCmd.CommandText = "Select * From vi_CliCod " & Where & strORD
                SqlDbSelectCmd.CommandType = CommandType.Text
                SqlDbSelectCmd.Connection = SqlConnCli

                SqlAdapCli.Fill(dsClientiStampa.vi_CliCod)

                For Each row As dsClienti.vi_CliCodRow In dsClientiStampa.vi_CliCod
                    row.BeginEdit()
                    row.Ditta = AzReport
                    row.Filtri = myStrFiltri
                    row.EndEdit()
                Next

                dsClientiStampa.AcceptChanges()
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa clienti."
            StampaClienti = False
            Exit Function
        End Try
    End Function

    Public Function StampaECCliente(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsClienti1 As dsClienti, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrA As String, ByVal CodCliente As String, ByVal CodDitta As String) As Boolean
        Dim strErrore As String = ""
        Dim strSQL As String = ""
        Dim rowDitta As dsClienti.DittaRow

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConnIn As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim SqlAdapIn As SqlDataAdapter
        Dim SqlDbSelectCmdIn As SqlCommand

        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
        SqlConnIn = New SqlConnection
        SqlAdapIn = New SqlDataAdapter
        SqlDbSelectCmdIn = New SqlCommand

        Try

            'giu190617
            Dim strValore As String = ""
            'Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            SqlDbSelectCmd.CommandTimeout = myTimeOUT
            SqlDbSelectCmdIn.CommandTimeout = myTimeOUT
            '---------------------------
            'Riempio DSClienti
            SqlAdap.SelectCommand = SqlDbSelectCmd
            SqlAdapIn.SelectCommand = SqlDbSelectCmdIn
            SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
            SqlConnIn.ConnectionString = dbCon.getConnectionString(TipoDB.dbInstall)

            SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectCmdIn.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectCmd.Connection = SqlConn1
            SqlDbSelectCmdIn.Connection = SqlConnIn

            SqlDbSelectCmd.CommandText = "get_Azi_EstrContoCliPerStampa"
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AData", SqlDbType.NVarChar, 10, ParameterDirection.Input, False, 0, 0, "", DataRowVersion.Current, myStrA))
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente", SqlDbType.NVarChar, 16, ParameterDirection.Input, False, 0, 0, "", DataRowVersion.Current, CodCliente))

            SqlDbSelectCmdIn.CommandText = "get_DitteByCodice"
            SqlDbSelectCmdIn.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Codice", SqlDbType.NVarChar, 8, ParameterDirection.Input, False, 0, 0, "", DataRowVersion.Current, CodDitta))

            dsClienti1.EstrContoPerStampa.Clear()
            dsClienti1.Ditta.Clear()

            'Riempio Dataset
            SqlAdap.Fill(dsClienti1.EstrContoPerStampa)
            SqlAdapIn.Fill(dsClienti1.Ditta)

            rowDitta = dsClienti1.Ditta(0)

            'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe

            Dim r_OrdPerCli As dsClienti.EstrContoPerStampaRow
            For Each r_OrdPerCli In dsClienti1.EstrContoPerStampa
                r_OrdPerCli.AziendaReport = strNomeAz
                r_OrdPerCli.TitoloReport = TitoloReport
                r_OrdPerCli.Titolo = TitoloReport 'giu010113
                r_OrdPerCli.Ditta = rowDitta.Descrizione
                r_OrdPerCli.Ditta_Indirizzo = IIf(IsDBNull(rowDitta.Indirizzo), "", rowDitta.Indirizzo)
                r_OrdPerCli.Ditta_Cap = IIf(IsDBNull(rowDitta.CAP), "", rowDitta.CAP)
                r_OrdPerCli.Ditta_Fax = IIf(IsDBNull(rowDitta.Fax), "", rowDitta.Fax)
                r_OrdPerCli.Ditta_Localita = IIf(IsDBNull(rowDitta.Citta), "", rowDitta.Citta)
                r_OrdPerCli.Ditta_Provincia = IIf(IsDBNull(rowDitta.Provincia), "", rowDitta.Provincia)
                r_OrdPerCli.Ditta_Telefono = IIf(IsDBNull(rowDitta.Telefono), "", rowDitta.Telefono)
            Next
            dsClienti1.AcceptChanges()

            Return True
        Catch ex As Exception
            Errore = ex.Message & " - Stampa clienti."
            StampaECCliente = False
            Exit Function
        End Try
    End Function

    Public Function StampaSCCliente(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsClienti1 As dsClienti, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, ByVal CodCliente As String, ByVal CodDitta As String) As Boolean
        Dim strErrore As String = ""
        Dim SQLstr As String = ""
        Dim rowSC As dsClienti.ScCont_VisRow
        Dim Dare As Decimal
        Dim Avere As Decimal

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConnAP As SqlConnection
        Dim SqlConnIn As SqlConnection
        Dim SqlConnCoGe As SqlConnection
        Dim SQLAdapEser As SqlDataAdapter
        Dim SQLAdapAP As SqlDataAdapter
        Dim SQLAdapCli As SqlDataAdapter
        Dim SQLAdapFor As SqlDataAdapter
        Dim SQLAdapPdc As SqlDataAdapter
        Dim SQLCmdEser As SqlCommand
        Dim SQLCmdAP As SqlCommand
        Dim SQLCmdCli As SqlCommand
        Dim SQLCmdFor As SqlCommand
        Dim SQLCmdPdc As SqlCommand

        SqlConnAP = New SqlConnection
        SqlConnIn = New SqlConnection
        SqlConnCoGe = New SqlConnection
        SQLAdapEser = New SqlDataAdapter
        SQLAdapAP = New SqlDataAdapter
        SQLAdapCli = New SqlDataAdapter
        SQLAdapFor = New SqlDataAdapter
        SQLAdapPdc = New SqlDataAdapter
        SQLCmdEser = New SqlCommand
        SQLCmdAP = New SqlCommand
        SQLCmdCli = New SqlCommand
        SQLCmdFor = New SqlCommand
        SQLCmdPdc = New SqlCommand

        Try
            'giu190617
            Dim strValore As String = ""
            'Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            SQLCmdEser.CommandTimeout = myTimeOUT
            SQLCmdAP.CommandTimeout = myTimeOUT
            '---------------------------
            SqlConnIn.ConnectionString = dbCon.getConnectionString(TipoDB.dbInstall)
            dsClienti1.Esercizi.Clear()
            SQLAdapEser.SelectCommand = SQLCmdEser
            SQLCmdEser.Connection = SqlConnIn
            SQLCmdEser.CommandType = CommandType.Text
            'SQLCmdEser.CommandText = "SELECT [Ditta], [Esercizio] FROM [Esercizi] WHERE Ditta = '" & CodDitta & "' AND CAST([Esercizio] AS int) >= " & Year(CDate(myStrDa.Trim)) _ alb060313
            '& " AND CAST([Esercizio] AS int) <= " & Year(CDate(myStrA.Trim)) & " ORDER BY [Esercizio] DESC"
            SQLCmdEser.CommandText = "SELECT [Ditta], [Esercizio] FROM [Esercizi] WHERE Ditta = '" & CodDitta & "' ORDER BY [Esercizio] DESC"

            SQLAdapEser.Fill(dsClienti1.Esercizi)

            If dsClienti1.Esercizi.Rows.Count = 0 Then
                StampaSCCliente = True
                Exit Function
            End If

            dsClienti1.ScCont_Vis.Clear()

            For Each RigaEser In dsClienti1.Esercizi.Rows
                SqlConnAP.ConnectionString = dbCon.getConnectionString(It.SoftAzi.Integration.Dao.DataSource.TipoConnessione.dbSoftCoge, RigaEser.Esercizio)
                SQLstr = "SELECT DISTINCT MovCont.Codice_CoGe, MovCont.Data_Reg, MovCont.N_Reg, " _
                & "MovCont.N_Riga, MovCont.Sezione, MovCont.Data_Doc, MovCont.N_Doc, MovCont.Des_Causale, "
                SQLstr = SQLstr & "(CASE [MovCont].[Sezione] WHEN 'D' THEN [MovCont].[Importo] ELSE 0 END) AS Dare, " _
                & "(CASE [MovCont].[Sezione] WHEN 'A' THEN [MovCont].[Importo] ELSE 0 END) AS Avere, "
                SQLstr = SQLstr & "0 AS Saldo, MovCont.Contro_Partita, ClientiCP.Rag_Soc AS Des_ClienteCP, FornitoriCP.Rag_Soc AS Des_FornitCP, " _
                & "PianoDeiContiCP.Descrizione AS Des_PdCCP, MovCont.N_IVA, '" & RigaEser.Esercizio & "' AS Esercizio, Autotrasporti, Stornata, " _
                & "Clienti.Rag_Soc AS Descrizione, Clienti.Indirizzo, Clienti.CAP, Clienti.Localita, Clienti.Telefono1 " _
                & "FROM MovCont INNER JOIN Clienti ON MovCont.Codice_CoGe = Clienti.Codice_CoGe LEFT OUTER JOIN " _
                & "Clienti AS ClientiCP ON MovCont.Contro_Partita = ClientiCP.Codice_CoGe LEFT OUTER JOIN " _
                & "Fornitori AS FornitoriCP ON MovCont.Contro_Partita = FornitoriCP.Codice_CoGe LEFT OUTER JOIN " _
                & "PianoDeiConti AS PianoDeiContiCP ON MovCont.Contro_Partita = PianoDeiContiCP.Codice_CoGe " _
                & "Where MovCont.Codice_CoGe = '" & CodCliente & "' "
                If Year(CDate(myStrA.Trim)) = CInt(RigaEser.Esercizio) Then
                    SQLstr = SQLstr & "AND MovCont.Data_Reg <= CONVERT(datetime, '" & myStrA.Trim & "', 103) "
                End If

                SQLAdapAP.SelectCommand = SQLCmdAP
                SQLCmdAP.Connection = SqlConnAP
                SQLCmdAP.CommandType = CommandType.Text
                SQLCmdAP.CommandText = SQLstr

                SQLAdapAP.Fill(dsClienti1.ScCont_Vis)

                SqlConnAP.Close()
            Next

            Dare = 0
            Avere = 0
            For Each rowSC In dsClienti1.ScCont_Vis.Rows
                rowSC.BeginEdit()
                rowSC.Decimali = 2
                rowSC.Des_Valuta = "EURO"
                rowSC.AziendaReport = strNomeAz
                rowSC.TitoloReport = TitoloReport
                rowSC.Ditta = strNomeAz
                rowSC.DataDa = myStrDa
                rowSC.DataA = myStrA
                rowSC.EndEdit()
                If rowSC.Data_Reg < CDate(myStrDa) Then
                    Dare = Dare + rowSC.Dare
                    Avere = Avere + rowSC.Avere
                    rowSC.Delete()
                End If
            Next

            dsClienti1.AcceptChanges()
            If dsClienti1.ScCont_Vis.Rows.Count = 0 Then
                StampaSCCliente = True
                Exit Function
            End If

            rowSC = dsClienti1.ScCont_Vis.NewScCont_VisRow

            With rowSC
                .BeginEdit()
                .Data_Reg = CDate(myStrDa).AddDays(-1)
                .N_Reg = 0
                .N_Riga = 1
                If Dare > Avere Then
                    .Sezione = "D"
                    .Dare = Dare - Avere
                    .Avere = 0
                Else
                    .Sezione = "A"
                    .Dare = 0
                    .Avere = Avere - Dare
                End If
                .N_IVA = 0
                .SetData_DocNull()
                .SetN_DocNull()
                .Des_Causale = "SALDO PRECEDENTE"
                .SetContro_PartitaNull()
                .SetDes_ClienteCPNull()
                .SetDes_FornitCPNull()
                .SetDes_PdCCPNull()
                .Codice_CoGe = CodCliente
                .Decimali = 2
                .Des_Valuta = "EURO"
                .Ditta = strNomeAz
                .AziendaReport = strNomeAz
                .TitoloReport = TitoloReport
                .DataDa = myStrDa
                .DataA = myStrA
                'alb060313 stampa più veloce (solo una tabella)
                If dsClienti1.ScCont_Vis.Rows(0).IsNull("Descrizione") Then
                    .SetDescrizioneNull()
                Else
                    .Descrizione = dsClienti1.ScCont_Vis.Rows(0)("Descrizione")
                End If

                If dsClienti1.ScCont_Vis.Rows(0).IsNull("Indirizzo") Then
                    .SetIndirizzoNull()
                Else
                    .Indirizzo = dsClienti1.ScCont_Vis.Rows(0)("Indirizzo")
                End If

                If dsClienti1.ScCont_Vis.Rows(0).IsNull("CAP") Then
                    .SetCAPNull()
                Else
                    .CAP = dsClienti1.ScCont_Vis.Rows(0)("CAP")
                End If

                If dsClienti1.ScCont_Vis.Rows(0).IsNull("Localita") Then
                    .SetLocalitaNull()
                Else
                    .Localita = dsClienti1.ScCont_Vis.Rows(0)("Localita")
                End If

                If dsClienti1.ScCont_Vis.Rows(0).IsNull("Telefono1") Then
                    .SetTelefono1Null()
                Else
                    .Telefono1 = dsClienti1.ScCont_Vis.Rows(0)("Telefono1")
                End If
                'alb060313 END

                .EndEdit()
            End With
            dsClienti1.ScCont_Vis.AddScCont_VisRow(rowSC)
            rowSC = Nothing

            dsClienti1.Clienti.Clear()
            dsClienti1.Fornitori.Clear()
            dsClienti1.PianoDeiConti.Clear()

            ' ''SqlConnCoGe.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)

            ' ''SQLAdapCli.SelectCommand = SQLCmdCli
            ' ''SQLCmdCli.Connection = SqlConnCoGe
            ' ''SQLCmdCli.CommandType = CommandType.StoredProcedure
            ' ''SQLCmdCli.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.Int, 4, ParameterDirection.ReturnValue, True, 0, 0, "", DataRowVersion.Current, Nothing))
            ' ''SQLCmdCli.CommandText = "get_Clienti"

            ' ''SQLAdapCli.Fill(dsClienti1.Clienti)

            ' ''SQLAdapFor.SelectCommand = SQLCmdFor
            ' ''SQLCmdFor.Connection = SqlConnCoGe
            ' ''SQLCmdFor.CommandType = CommandType.StoredProcedure
            ' ''SQLCmdFor.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.Int, 4, ParameterDirection.ReturnValue, True, 0, 0, "", DataRowVersion.Current, Nothing))
            ' ''SQLCmdFor.CommandText = "get_Fornitori"

            ' ''SQLAdapFor.Fill(dsClienti1.Fornitori)

            ' ''SQLAdapPdc.SelectCommand = SQLCmdPdc
            ' ''SQLCmdPdc.Connection = SqlConnCoGe
            ' ''SQLCmdPdc.CommandType = CommandType.StoredProcedure
            ' ''SQLCmdPdc.Parameters.Add(New SqlParameter("@RETURN_VALUE", SqlDbType.Int, 4, ParameterDirection.ReturnValue, True, 0, 0, "", DataRowVersion.Current, Nothing))
            ' ''SQLCmdPdc.CommandText = "get_PianoDeiConti"

            ' ''SQLAdapPdc.Fill(dsClienti1.PianoDeiConti)

            'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe

            'For Each rowSC In dsClienti1.ScCont_Vis.Rows
            '    rowSC.Decimali = 2
            '    rowSC.Des_Valuta = "EURO"
            '    rowSC.AziendaReport = strNomeAz
            '    rowSC.TitoloReport = TitoloReport
            '    rowSC.Ditta = strNomeAz
            '    rowSC.DataDa = myStrDa
            '    rowSC.DataA = myStrA
            'Next
            ' ''dsClienti1.AcceptChanges()

            Return True
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Scheda Contabile clienti."
            StampaSCCliente = False
            Exit Function
        End Try
    End Function
End Class
