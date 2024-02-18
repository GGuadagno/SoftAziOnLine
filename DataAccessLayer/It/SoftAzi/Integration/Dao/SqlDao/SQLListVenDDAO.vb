Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil

Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLListVenDDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.ListVenDDAO

        Public Function getListVenDByCodLisCodArt(ByVal idTransazione As Integer, ByVal CodLis As Integer, ByVal CodArt As String) As System.Collections.ArrayList Implements ListVenDDAO.getListVenDByCodLisCodArt
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myListVenD As ListVenDEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_ListVenDByCodLisCodArt"
                '-- CodLis
                param = theSqlCommand.Parameters.Add("@CodLis", SqlDbType.Int)
                param.Direction = ParameterDirection.Input
                param.Value = CodLis
                '-- CodArt
                param = theSqlCommand.Parameters.Add("@CodArt", SqlDbType.NVarChar, 20)
                param.Direction = ParameterDirection.Input
                param.Value = CodArt
                '--
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myListVenD = New ListVenDEntity
                            With myListVenD
                                .Codice = getIntegerParameter(theSqlDataReader, 0)
                                .Riga = getIntegerParameter(theSqlDataReader, 1)
                                .Cod_Articolo = getStringParameter(theSqlDataReader, 2)
                                .Descrizione = getStringParameter(theSqlDataReader, 3)
                                .Prezzo = getDecimalParameter(theSqlDataReader, 4)
                                .Prezzo_Valuta = getDecimalParameter(theSqlDataReader, 5)
                                .Sconto_1 = getDecimalParameter(theSqlDataReader, 6)
                                .Sconto_2 = getDecimalParameter(theSqlDataReader, 7)
                                .PrezzoMinimo = getDecimalParameter(theSqlDataReader, 8)
                                .Data_Cambio = getDataTimeParameter(theSqlDataReader, 9)
                                .Cambio = getDecimalParameter(theSqlDataReader, 10)
                            End With

                            myArray.Add(myListVenD)
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
        Public Function InsertUpdateListVenD(ByVal idTransazione As Integer, ByVal paramListVenD As Object) As Boolean Implements ListVenDDAO.InsertUpdateListVenD
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim theError As Integer
            Dim bReturnOk As Boolean
            Dim myListVenDEntity As ListVenDEntity = CType(paramListVenD, ListVenDEntity)

            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "InsertUpdate_ListVenD"
                param = setParameter(param, theSqlCommand, "@CodLis", SqlDbType.Int, myListVenDEntity.Codice)
                param = setParameter(param, theSqlCommand, "@Cod_Articolo", SqlDbType.NVarChar, myListVenDEntity.Cod_Articolo, 20)
                param = setParameter(param, theSqlCommand, "@Prezzo", SqlDbType.Decimal, myListVenDEntity.Prezzo) 'giu110512 , 9)
                param = setParameter(param, theSqlCommand, "@Sconto_1", SqlDbType.Decimal, myListVenDEntity.Sconto_1) 'giu110512 , 9)
                param = setParameter(param, theSqlCommand, "@Sconto_2", SqlDbType.Decimal, myListVenDEntity.Sconto_2) 'giu110512 , 9)
                param = setParameter(param, theSqlCommand, "@PrezzoMinimo", SqlDbType.Decimal, myListVenDEntity.PrezzoMinimo) 'giu110512 , 9)
                param = theSqlCommand.Parameters.Add("@RetVal", SqlDbType.Int)
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

        Public Function InsertiArticoliOu(ByVal idTransazione As Integer, ByVal CodLis As Integer) As Boolean Implements ListVenDDAO.InsertiArticoliOu
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim theError As Integer
            Dim bReturnOk As Boolean

            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "Insert_ArticoliEsclusi"
                param = setParameter(param, theSqlCommand, "@CodLis", SqlDbType.Int, CodLis)
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
