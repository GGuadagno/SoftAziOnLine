Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLUltimiPrezziAcquistoDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.UltimiPrezziAcquistoDAO

        Public Function getUltimiPrezziAcquistoByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As System.Collections.ArrayList Implements UltimiPrezziAcquistoDAO.getUltimiPrezziAcquistoByCodiceArticolo
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myUltimiPrezziAcquisto As UltimiPrezziAcquistoEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_UltimiPrezziAcquistoByCodiceArticolo"
                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.NVarChar, 20)
                param.Direction = ParameterDirection.Input
                param.Value = Codice
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myUltimiPrezziAcquisto = New UltimiPrezziAcquistoEntity
                            With myUltimiPrezziAcquisto
                                .CodArticolo = getStringParameter(theSqlDataReader, 0)
                                .DataAcquisto = getDataTimeParameter(theSqlDataReader, 1)
                                .Prezzo = getDecimalParameter(theSqlDataReader, 2)
                            End With

                            myArray.Add(myUltimiPrezziAcquisto)
                        End While
                    Else
                        myArray = Nothing
                    End If
                    theSqlDataReader.Close()
                    Return myArray
                Catch sex As SqlException
                    If Not IsNothing(theSqlDataReader) Then
                        theSqlDataReader.Close()
                    End If
                    Throw sex
                End Try
            Catch exf As Exception
                If Not IsNothing(theSqlDataReader) Then
                    theSqlDataReader.Close()
                End If
                Throw exf
            End Try
        End Function
        Public Function InsertUltimiPrezziAcquisto(ByVal idTransazione As Integer, ByVal paramUltPrezzoAcquisto As Object) As Boolean Implements UltimiPrezziAcquistoDAO.InsertUltimiPrezziAcquisto
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim theError As Integer
            Dim bReturnOk As Boolean
            Dim myUltPrezzoAcquistoEntity As UltimiPrezziAcquistoEntity = CType(paramUltPrezzoAcquisto, UltimiPrezziAcquistoEntity)

            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "Insert_UltimiPrezziAcquisto"
                param = setParameter(param, theSqlCommand, "@Cod_Articolo", SqlDbType.NVarChar, myUltPrezzoAcquistoEntity.CodArticolo, 20)
                param = setParameter(param, theSqlCommand, "@Prezzo", SqlDbType.Decimal, myUltPrezzoAcquistoEntity.Prezzo, 9)
                'param = setParameter(param, theSqlCommand, "@DataAcquisto", SqlDbType.Int, myUltPrezzoAcquistoEntity.DataAcquisto, 8)
                param = theSqlCommand.Parameters.Add("@RetVal", SqlDbType.Int, 4)
                param.Direction = ParameterDirection.Output
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = -1 Then
                        bReturnOk = False
                    Else
                        bReturnOk = True
                    End If
                    theSqlDataReader.Close()
                    Return bReturnOk
                Catch sex As SqlException
                    If Not IsNothing(theSqlDataReader) Then
                        theSqlDataReader.Close()
                    End If
                    Throw sex
                End Try
            Catch exf As Exception
                If Not IsNothing(theSqlDataReader) Then
                    theSqlDataReader.Close()
                End If
                Throw exf
            End Try
        End Function

    End Class
End Namespace
