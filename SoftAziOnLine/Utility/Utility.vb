Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Public Class Utility


    Public Function Impegna_Giacenze(ByVal CodiceArt As String, ByRef strErrore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnImp As SqlConnection
        Dim SqlImpegnaGiacenzaOrdiniClienti As SqlCommand
        Try
            SqlConnImp = New SqlConnection
            SqlImpegnaGiacenzaOrdiniClienti = New SqlCommand

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
            SqlImpegnaGiacenzaOrdiniClienti.CommandTimeout = myTimeOUT
            '---------------------------
            SqlConnImp.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlImpegnaGiacenzaOrdiniClienti.CommandType = System.Data.CommandType.StoredProcedure
            SqlImpegnaGiacenzaOrdiniClienti.CommandText = "set_ImpegnaGiacenzaOrdiniClienti"
            SqlImpegnaGiacenzaOrdiniClienti.Connection = SqlConnImp
            SqlImpegnaGiacenzaOrdiniClienti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            If CodiceArt = "" Then
                SqlImpegnaGiacenzaOrdiniClienti.Parameters("@Cod_Articolo").Value = DBNull.Value
            Else
                SqlImpegnaGiacenzaOrdiniClienti.Parameters("@Cod_Articolo").Value = CodiceArt
            End If
            If SqlConnImp.State = ConnectionState.Closed Then
                SqlConnImp.Open()
            End If

            SqlImpegnaGiacenzaOrdiniClienti.ExecuteNonQuery()
        Catch ex As Exception
            strErrore = "(Impegna_Giacenze) Errore:" & ex.Message
            Return False
            Exit Function
        Finally
            If Not IsNothing(SqlConnImp) Then 'giu240120
                If SqlConnImp.State <> ConnectionState.Closed Then
                    SqlConnImp.Close()
                    SqlConnImp = Nothing
                End If
            End If
        End Try

        Return True

    End Function

    Public Shared Function InfoPerImpegnoGiacenzaOrdiniClienti(ByRef TotOrdini As Integer, ByRef DataPrimoOrdine As Date, ByRef DataUltimoOrdine As Date, ByRef strErrore As String)

        Try
            Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
            Dim SqlConnInfo = New System.Data.SqlClient.SqlConnection
            Dim SqlSel_Info = New System.Data.SqlClient.SqlCommand

            SqlSel_Info.CommandText = "[get_infoImpegnoGiacenzaOrdineClienti]"
            SqlSel_Info.CommandType = System.Data.CommandType.StoredProcedure
            SqlConnInfo.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlSel_Info.Connection = SqlConnInfo
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotOrdini", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataPrimoOrdine", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataUltimoOrdine", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))


            If SqlConnInfo.State = ConnectionState.Closed Then
                SqlConnInfo.Open()
            End If



            SqlSel_Info.ExecuteNonQuery()

            TotOrdini = SqlSel_Info.Parameters("@TotOrdini").Value
            If Not IsDBNull(SqlSel_Info.Parameters("@DataPrimoOrdine").Value) Then
                DataPrimoOrdine = SqlSel_Info.Parameters("@DataPrimoOrdine").Value
            Else
                DataPrimoOrdine = CDate("01/01/1900")
            End If

            If Not IsDBNull(SqlSel_Info.Parameters("@DataUltimoOrdine").Value) Then
                DataUltimoOrdine = SqlSel_Info.Parameters("@DataUltimoOrdine").Value
            Else
                DataUltimoOrdine = CDate("01/01/1900")
            End If

            If Not IsNothing(SqlConnInfo) Then 'giu240120
                If SqlConnInfo.State <> ConnectionState.Closed Then
                    SqlConnInfo.Close()
                    SqlConnInfo = Nothing
                End If
            End If

            Return True
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        End Try
    End Function

End Class
