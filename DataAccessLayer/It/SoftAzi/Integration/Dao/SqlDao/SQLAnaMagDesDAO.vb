Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLAnaMagDesDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.AnaMagDesDAO

        Public Function getAnaMagDesByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As System.Collections.ArrayList Implements AnaMagDesDAO.getAnaMagDesByCodiceArticolo
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myAnaMagDes As AnaMagDesEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_AnaMagDesByCodiceArticolo"
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
                            myAnaMagDes = New AnaMagDesEntity
                            With myAnaMagDes
                                .CodArticolo = getStringParameter(theSqlDataReader, 0)
                                .Progressivo = getIntegerParameter(theSqlDataReader, 1)
                                .Descrizione = getStringParameter(theSqlDataReader, 2)
                            End With

                            myArray.Add(myAnaMagDes)
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
        Public Function InsertAnaMagDes(ByVal idTransazione As Integer, ByVal paramAnaMagDes As Object) As Boolean Implements AnaMagDesDAO.InsertAnaMagDes
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim theError As Integer
            Dim bReturnOk As Boolean
            Dim myAnaMagDesEntity As AnaMagDesEntity = CType(paramAnaMagDes, AnaMagDesEntity)

            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "Insert_AnaMagDes"
                param = setParameter(param, theSqlCommand, "@Cod_Articolo", SqlDbType.NVarChar, myAnaMagDesEntity.CodArticolo, 20)
                param = setParameter(param, theSqlCommand, "@Descrizione", SqlDbType.NText, myAnaMagDesEntity.Descrizione)
                param = setParameter(param, theSqlCommand, "@Progressivo", SqlDbType.Int, myAnaMagDesEntity.Progressivo, 4)
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
        Public Function delAnaMagDesByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements AnaMagDesDAO.delAnaMagDesByCodiceArticolo
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim bReturnOk As Boolean
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "del_AnaMagDesByCodiceArticolo"
                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.NVarChar, 20)
                param.Direction = ParameterDirection.Input
                param.Value = Codice
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
