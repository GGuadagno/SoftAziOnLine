Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil

Namespace It.SoftAzi.Integration.Dao.SQLDao

    Public Class SQLOperatoriDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.OperatoriDAO

        Public Function getOperatoriByName(ByVal idTransazione As Integer, ByVal nome As String) As System.Collections.ArrayList Implements OperatoriDAO.getOperatoriByName
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myOperatore As OperatoriEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_OperatoriByName"
                param = theSqlCommand.Parameters.Add("@nome", SqlDbType.NVarChar, 12)
                param.Direction = ParameterDirection.Input
                param.Value = nome
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myOperatore = New OperatoriEntity
                            With myOperatore
                                .Codice = getStringParameter(theSqlDataReader, 0)
                                .Nome = getStringParameter(theSqlDataReader, 1)
                                .Password = getStringParameter(theSqlDataReader, 2)
                                .Livello = getStringParameter(theSqlDataReader, 3)
                                .Analitico = getIntegerParameter(theSqlDataReader, 4)
                                .RIBA = getIntegerParameter(theSqlDataReader, 5)
                                .Contatore = getIntegerParameter(theSqlDataReader, 6)
                                .Mod_Coge = getIntegerParameter(theSqlDataReader, 7)
                                .Mod_Azi = getIntegerParameter(theSqlDataReader, 8)
                                .Mod_Fatture = getIntegerParameter(theSqlDataReader, 9)
                                .Mod_Contratti = getIntegerParameter(theSqlDataReader, 10)
                                .Mod_Hotel = getIntegerParameter(theSqlDataReader, 11)
                                .Mod_Rist = getIntegerParameter(theSqlDataReader, 12)
                                .DataScadenza = getDataTimeParameter(theSqlDataReader, 13)
                                .NessunaScadenza = getBooleanParameter(theSqlDataReader, 14)
                                .CodiceDitta = getStringParameter(theSqlDataReader, 15)
                                .DataOraUltimoAccesso = getDataTimeParameter(theSqlDataReader, 16)
                                .PwdSblocca = getStringParameter(theSqlDataReader, 17)
                            End With

                            myArray.Add(myOperatore)
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

        Public Function UpdOperatoriDataOraUltAccesso(ByVal idTransazione As Integer, ByVal Nome As String) As Boolean Implements OperatoriDAO.UpdOperatoriDataOraUltAccesso
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
                theSqlCommand.CommandText = "UpdOperatoriDataOraUltAccesso"
                param = theSqlCommand.Parameters.Add("@Nome", SqlDbType.NVarChar, 12)
                param.Direction = ParameterDirection.Input
                param.Value = Nome
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        bReturnOk = True
                    Else
                        bReturnOk = False
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

        Public Function OperatoreConnesso(ByVal idTransazione As Integer, ByVal Nome As String, ByVal CodiceDitta As String, ByVal Postazione As String, ByVal Modulo As String, ByVal SessionID As String, ByVal _ID As Integer, ByVal Codice As String, ByVal Azienda As String, ByVal Tipo As String, ByVal _Esercizio As String) As System.Collections.ArrayList Implements OperatoriDAO.OperatoreConnesso
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myOperatoreConnesso As OperatoreConnessoEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "Web_GetOperatoreConesso"
                param = theSqlCommand.Parameters.Add("@NomeOperatore", SqlDbType.NVarChar, 12)
                param.Direction = ParameterDirection.Input
                param.Value = Nome

                param = theSqlCommand.Parameters.Add("@CodiceDitta", SqlDbType.NVarChar, 8)
                param.Direction = ParameterDirection.Input
                param.Value = CodiceDitta

                param = theSqlCommand.Parameters.Add("@Postazione", SqlDbType.NVarChar, 50)
                param.Direction = ParameterDirection.Input
                param.Value = Postazione

                param = theSqlCommand.Parameters.Add("@Modulo", SqlDbType.NVarChar, 10)
                param.Direction = ParameterDirection.Input
                param.Value = Modulo

                param = theSqlCommand.Parameters.Add("@SessionID", SqlDbType.NVarChar, 50)
                param.Direction = ParameterDirection.Input
                param.Value = SessionID
                'giu050312
                param = theSqlCommand.Parameters.Add("@ID", SqlDbType.Int)
                param.Direction = ParameterDirection.Input
                param.Value = _ID

                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.NVarChar, 16)
                param.Direction = ParameterDirection.Input
                param.Value = Codice

                param = theSqlCommand.Parameters.Add("@Azienda", SqlDbType.NVarChar, 50)
                param.Direction = ParameterDirection.Input
                param.Value = Azienda

                param = theSqlCommand.Parameters.Add("@Tipo", SqlDbType.NVarChar, 50)
                param.Direction = ParameterDirection.Input
                param.Value = Tipo

                param = theSqlCommand.Parameters.Add("@Esercizio", SqlDbType.NVarChar, 4)
                param.Direction = ParameterDirection.Input
                param.Value = _Esercizio

                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myOperatoreConnesso = New OperatoreConnessoEntity
                            With myOperatoreConnesso
                                .NomeOperatore = getStringParameter(theSqlDataReader, 0)
                                .CodiceDitta = getStringParameter(theSqlDataReader, 1)
                                .Postazione = getStringParameter(theSqlDataReader, 2)
                                .Modulo = getStringParameter(theSqlDataReader, 3)
                                .SessionID = getStringParameter(theSqlDataReader, 4)
                                .ID = getIntegerParameter(theSqlDataReader, 5)
                                .Codice = getStringParameter(theSqlDataReader, 6)
                                .Azienda = getStringParameter(theSqlDataReader, 7)
                                .Tipo = getStringParameter(theSqlDataReader, 8)
                                .Esercizio = getStringParameter(theSqlDataReader, 9)
                            End With

                            myArray.Add(myOperatoreConnesso)
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

        Public Function DelOperatoreConnesso(ByVal idTransazione As Integer, ByVal Nome As String, ByVal CodiceDitta As String, ByVal Modulo As String) As Boolean Implements OperatoriDAO.DelOperatoreConnesso
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
                theSqlCommand.CommandText = "del_OperatoriConnessi"
                param = theSqlCommand.Parameters.Add("@NomeOperatore", SqlDbType.NVarChar, 12)
                param.Direction = ParameterDirection.Input
                param.Value = Nome

                param = theSqlCommand.Parameters.Add("@CodiceDitta", SqlDbType.NVarChar, 8)
                param.Direction = ParameterDirection.Input
                param.Value = CodiceDitta

                param = theSqlCommand.Parameters.Add("@Modulo", SqlDbType.NVarChar, 10)
                param.Direction = ParameterDirection.Input
                param.Value = Modulo

                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 1 Then
                        bReturnOk = True
                    Else
                        bReturnOk = False
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

        Public Function UpdOperatoriDataOraAzione(ByVal idTransazione As Integer, ByVal Nome As String) As Boolean Implements OperatoriDAO.UpdOperatoriDataOraUltAzione
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
                theSqlCommand.CommandText = "UpdOperatoriDataOraUltAzione"
                param = theSqlCommand.Parameters.Add("@Nome", SqlDbType.NVarChar, 12)
                param.Direction = ParameterDirection.Input
                param.Value = Nome
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        bReturnOk = True
                    Else
                        bReturnOk = False
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