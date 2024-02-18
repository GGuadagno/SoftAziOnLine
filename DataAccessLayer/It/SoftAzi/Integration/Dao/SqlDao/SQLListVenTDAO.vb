Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil

Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLListVenTDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.ListVenTDAO

        Public Function getListVenTByCodice(ByVal idTransazione As Integer, ByVal Codice As Integer) As System.Collections.ArrayList Implements ListVenTDAO.getListVenTByCodice
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myListVenT As ListVenTEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_ListVenTByCodice"
                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.Int)
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
                            myListVenT = New ListVenTEntity
                            With myListVenT
                                .Codice = getIntegerParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .Tipo = getStringParameter(theSqlDataReader, 2)
                                .Data_Inizio_Validita = getDataTimeParameter(theSqlDataReader, 3)
                                .Abilitato = getBooleanParameter(theSqlDataReader, 4)
                                .Valuta = getStringParameter(theSqlDataReader, 5)
                                .Cod_Pagamento = getIntegerParameter(theSqlDataReader, 6)
                                .Categoria = getIntegerParameter(theSqlDataReader, 7)
                                .Cliente = getStringParameter(theSqlDataReader, 8)
                                .Note = getStringParameter(theSqlDataReader, 9)
                            End With

                            myArray.Add(myListVenT)
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

        Public Function getListVenT(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements ListVenTDAO.getListVenT
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myListVenT As ListVenTEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_ListVenT"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myListVenT = New ListVenTEntity
                            With myListVenT
                                .Codice = getIntegerParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .Tipo = getStringParameter(theSqlDataReader, 2)
                                .Data_Inizio_Validita = getDataTimeParameter(theSqlDataReader, 3)
                                .Abilitato = getBooleanParameter(theSqlDataReader, 4)
                                .Valuta = getStringParameter(theSqlDataReader, 5)
                                .Cod_Pagamento = getIntegerParameter(theSqlDataReader, 6)
                                .Categoria = getIntegerParameter(theSqlDataReader, 7)
                                .Cliente = getStringParameter(theSqlDataReader, 8)
                                .Note = getStringParameter(theSqlDataReader, 9)
                            End With

                            myArray.Add(myListVenT)
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
    End Class
End Namespace