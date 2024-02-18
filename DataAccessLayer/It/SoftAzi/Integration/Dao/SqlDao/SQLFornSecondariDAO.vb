Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLFornSecondariDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.FornSecondariDAO

        Public Function getFornSecondariByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As System.Collections.ArrayList Implements FornSecondariDAO.getFornSecondariByCodiceArticolo
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myFornSecondari As FornSecondariEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_FornSecondariByCodiceArticolo"
                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.NVarChar, 8)
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
                            myFornSecondari = New FornSecondariEntity
                            With myFornSecondari
                                .CodArticolo = getStringParameter(theSqlDataReader, 0)
                                .CodFornitore = getStringParameter(theSqlDataReader, 1)
                                .CodPagamento = getIntegerParameter(theSqlDataReader, 2)
                                .GiorniConsegna = getIntegerParameter(theSqlDataReader, 3)
                                .UltPrezzo = getDecimalParameter(theSqlDataReader, 4)
                                .RagSoc = getStringParameter(theSqlDataReader, 5)
                                .Titolare = getStringParameter(theSqlDataReader, 6)
                                .Riferimento = getStringParameter(theSqlDataReader, 7)
                            End With
                            myArray.Add(myFornSecondari)
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
        Public Function delFornSecByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements FornSecondariDAO.delFornSecByCodiceArticolo
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
                theSqlCommand.CommandText = "del_FornSecondariByCodiceArticolo"
                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.NVarChar, 20)
                param.Direction = ParameterDirection.Input
                param.Value = Codice
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        bReturnOk = True
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
        Public Function InsertFornitoriSec(ByVal idTransazione As Integer, ByVal paramFornSec As Object) As Boolean Implements FornSecondariDAO.InsertFornitoriSec
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim theError As Integer
            Dim bReturnOk As Boolean
            Dim myFornSecEntity As FornSecondariEntity = CType(paramFornSec, FornSecondariEntity)

            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "Insert_FornitoriSec"
                param = setParameter(param, theSqlCommand, "@Cod_Articolo", SqlDbType.NVarChar, myFornSecEntity.CodArticolo, 20)
                param = setParameter(param, theSqlCommand, "@Cod_Fornitore", SqlDbType.NVarChar, myFornSecEntity.CodFornitore, 16)
                param = setParameter(param, theSqlCommand, "@Cod_pagamento", SqlDbType.Int, myFornSecEntity.CodPagamento)
                param = setParameter(param, theSqlCommand, "@Giorni_consegna", SqlDbType.Int, myFornSecEntity.GiorniConsegna)
                param = setParameter(param, theSqlCommand, "@UltPrezzo", SqlDbType.Decimal, myFornSecEntity.CodPagamento, 9)
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
    End Class
End Namespace
