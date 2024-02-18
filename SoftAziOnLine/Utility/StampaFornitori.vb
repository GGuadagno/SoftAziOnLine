Imports SoftAziOnLine.Def
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Public Class StampaFornitori
    Public Function StampaFornitori(ByVal AzReport As String, ByVal dsFornitoriStampa As dsClienti, ByRef Errore As String, ByVal TipoStampa As Integer, ByVal Ordinamento As String, ByVal Where As String, ByVal myStrFIltri As String, Optional ByVal dettagli As Integer = -1) As Boolean
        StampaFornitori = True
        Dim DBConn As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnFor As SqlConnection
        Dim SqlAdapFor As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strORD As String = ""
        Try
            'dsFornitoriStampa.Clear()
            If Ordinamento <> "" Then
                strORD = " ORDER BY " & Ordinamento
            End If
            If TipoStampa = 1 Then 'Or TipoStampa = 2 Then
                SqlConnFor = New SqlConnection(DBConn.getConnectionString(TipoDB.dbSoftCoge))

                SqlDbSelectCmd = New SqlCommand()
                SqlAdapFor = New SqlDataAdapter()

                SqlAdapFor.SelectCommand = SqlDbSelectCmd
                SqlDbSelectCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Indirizzo, Localita, CAP, " & _
                                             "Provincia, Nazione, Telefono1, Partita_IVA, ABI_N, CAB_N, " & _
                                             "Conto_Corrente, Descrizione, Banca, Filiale, Codice_Fiscale " & _
                                             "FROM vi_ForSint " & Where & strORD
                SqlDbSelectCmd.CommandType = CommandType.Text
                SqlDbSelectCmd.Connection = SqlConnFor
                SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlAdapFor.Fill(dsFornitoriStampa.vi_ForSint)
                For Each row As dsClienti.vi_ForSintRow In dsFornitoriStampa.vi_ForSint
                    'row.BeginEdit()
                    row.Ditta = AzReport
                    row.Filtri = myStrFIltri
                    'row.EndEdit()
                Next
                dsFornitoriStampa.AcceptChanges()
            ElseIf TipoStampa = 2 Then
                SqlConnFor = New SqlConnection(DBConn.getConnectionString(TipoDB.dbSoftCoge))

                SqlDbSelectCmd = New SqlCommand()
                SqlAdapFor = New SqlDataAdapter()

                Select Case dettagli
                    Case -1
                        SqlAdapFor.SelectCommand = SqlDbSelectCmd
                        SqlDbSelectCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Indirizzo, Localita, CAP, " & _
                                                     "Provincia, Nazione, Telefono1, Telefono2, Fax, " & _
                                                     "Partita_Iva " & _
                                                     "FROM vi_ForRub " & Where & strORD
                        SqlDbSelectCmd.CommandType = CommandType.Text
                        SqlDbSelectCmd.Connection = SqlConnFor
                        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                        SqlAdapFor.Fill(dsFornitoriStampa.vi_ForRub)
                        For Each row As dsClienti.vi_ForRubRow In dsFornitoriStampa.vi_ForRub
                            row.Ditta = AzReport
                            row.Filtri = myStrFIltri
                        Next
                        dsFornitoriStampa.AcceptChanges()
                        SqlDbSelectCmd = Nothing
                    Case 0
                        SqlAdapFor.SelectCommand = SqlDbSelectCmd
                        SqlDbSelectCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Indirizzo, Localita, CAP, " & _
                                                     "Provincia, Nazione, Telefono1, Telefono2, Fax, " & _
                                                     "Partita_Iva " & _
                                                     "FROM vi_ForRub " & Where & strORD
                        SqlDbSelectCmd.CommandType = CommandType.Text
                        SqlDbSelectCmd.Connection = SqlConnFor
                        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                        SqlAdapFor.Fill(dsFornitoriStampa.vi_ForRub)
                        For Each row As dsClienti.vi_ForRubRow In dsFornitoriStampa.vi_ForRub
                            row.Ditta = AzReport
                            row.Filtri = myStrFIltri
                        Next
                        dsFornitoriStampa.AcceptChanges()
                        SqlDbSelectCmd = Nothing
                    Case 1
                        SqlAdapFor.SelectCommand = SqlDbSelectCmd
                        SqlDbSelectCmd.CommandText = "SELECT DISTINCT vi_ForRub.Codice_CoGe, vi_ForRub.Rag_Soc, vi_ForRub.Indirizzo, vi_ForRub.Localita, vi_ForRub.CAP, " & _
                                                     "vi_ForRub.Provincia, vi_ForRub.Nazione, vi_ForRub.Telefono1, vi_ForRub.Telefono2, vi_ForRub.Fax, " & _
                                                     "vi_ForRub.Partita_Iva " & _
                                                     "FROM vi_ForRub INNER JOIN IndirCF ON vi_ForRub.Codice_CoGe = IndirCF.Codice" & Where & strORD
                        SqlDbSelectCmd.CommandType = CommandType.Text
                        SqlDbSelectCmd.Connection = SqlConnFor
                        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                        SqlAdapFor.Fill(dsFornitoriStampa.vi_ForRub)
                        For Each row As dsClienti.vi_ForRubRow In dsFornitoriStampa.vi_ForRub
                            row.Ditta = AzReport
                            row.Filtri = myStrFIltri
                        Next
                        dsFornitoriStampa.AcceptChanges()

                        SqlDbSelectCmd = Nothing

                        SqlDbSelectCmd = New SqlCommand("Select * from IndirCF", SqlConnFor)
                        SqlAdapFor.SelectCommand = SqlDbSelectCmd

                        SqlAdapFor.Fill(dsFornitoriStampa.IndirCF)
                    Case 2
                        SqlAdapFor.SelectCommand = SqlDbSelectCmd
                        SqlDbSelectCmd.CommandText = "SELECT vi_ForRub.Codice_CoGe, vi_ForRub.Rag_Soc, vi_ForRub.Indirizzo, vi_ForRub.Localita, vi_ForRub.CAP, " & _
                                                     "vi_ForRub.Provincia, vi_ForRub.Nazione, vi_ForRub.Telefono1, vi_ForRub.Telefono2, vi_ForRub.Fax, " & _
                                                     "vi_ForRub.Partita_Iva " & _
                                                     "FROM vi_ForRub INNER JOIN DestClienti ON vi_ForRub.Codice_CoGe = DestClienti.Codice" & Where & strORD
                        SqlDbSelectCmd.CommandType = CommandType.Text
                        SqlDbSelectCmd.Connection = SqlConnFor
                        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                        SqlAdapFor.Fill(dsFornitoriStampa.vi_ForRub)
                        For Each row As dsClienti.vi_ForRubRow In dsFornitoriStampa.vi_ForRub
                            row.Ditta = AzReport
                            row.Filtri = myStrFIltri
                        Next
                        dsFornitoriStampa.AcceptChanges()

                        SqlDbSelectCmd = Nothing

                        SqlDbSelectCmd = New SqlCommand("Select * from DestClienti", SqlConnFor)
                        SqlAdapFor.SelectCommand = SqlDbSelectCmd

                        SqlAdapFor.Fill(dsFornitoriStampa.DestClienti)
                End Select
            ElseIf TipoStampa = 3 Then
                SqlConnFor = New SqlConnection(DBConn.getConnectionString(TipoDB.dbSoftCoge))

                SqlDbSelectCmd = New SqlCommand()
                SqlAdapFor = New SqlDataAdapter()

                SqlAdapFor.SelectCommand = SqlDbSelectCmd
                SqlDbSelectCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Indirizzo, Localita, CAP, " & _
                                            "Provincia, Nazione, Telefono1, Telefono2, Fax, " & _
                                            "Codice_Fiscale, Partita_Iva, Zona, Categoria, " & _
                                            "ABI_N, CAB_N, Regime_IVA, Pagamento_N, " & _
                                            "DesBanca, DesFiliale, RegimeIVA, DesZona, DesPagamento, " & _
                                            "DesCateg " & _
                                            "FROM vi_ForAnalit " & Where & strORD
                SqlDbSelectCmd.CommandType = CommandType.Text
                SqlDbSelectCmd.Connection = SqlConnFor

                SqlAdapFor.Fill(dsFornitoriStampa.vi_ForAnalit)

                For Each row As dsClienti.vi_ForAnalitRow In dsFornitoriStampa.vi_ForAnalit
                    'row.BeginEdit()
                    row.Ditta = AzReport
                    row.Filtri = myStrFIltri
                    'row.EndEdit()
                Next

                dsFornitoriStampa.AcceptChanges()
            ElseIf TipoStampa = 4 Then
                SqlConnFor = New SqlConnection(DBConn.getConnectionString(TipoDB.dbSoftCoge))

                SqlDbSelectCmd = New SqlCommand()
                SqlAdapFor = New SqlDataAdapter()

                SqlAdapFor.SelectCommand = SqlDbSelectCmd
                SqlDbSelectCmd.CommandText = "SELECT Codice_CoGe, Rag_Soc, Zona, Categoria, " & _
                                             "ABI_N, CAB_N, Regime_IVA, Pagamento_N, Ragg_P, " & _
                                             "DesRaggP, DesZona, DesCateg, DesBanca, DesFiliale,  " & _
                                             "DesRegimeIVA, DesPagamento " & _
                                             "FROM vi_ForCod " & Where & strORD
                SqlDbSelectCmd.CommandType = CommandType.Text
                SqlDbSelectCmd.Connection = SqlConnFor

                SqlAdapFor.Fill(dsFornitoriStampa.vi_ForCod)

                For Each row As dsClienti.vi_ForCodRow In dsFornitoriStampa.vi_ForCod
                    ' row.BeginEdit()
                    row.Ditta = AzReport
                    row.Filtri = myStrFIltri
                    'row.EndEdit()
                Next

                dsFornitoriStampa.AcceptChanges()
            End If


        Catch ex As Exception
            Errore = ex.Message & " - Stampa fornitori."
            StampaFornitori = False
            Exit Function
        End Try

    End Function
End Class
