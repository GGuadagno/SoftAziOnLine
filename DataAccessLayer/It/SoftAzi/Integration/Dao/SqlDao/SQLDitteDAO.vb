Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil

Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLDitteDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.DitteDAO

        Public Function getDitteByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As System.Collections.ArrayList Implements DitteDAO.getDitteByCodice
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myDitta As DitteEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_DitteByCodice"
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
                            myDitta = New DitteEntity
                            With myDitta
                                .Codice = getStringParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .PartitaIVA = getStringParameter(theSqlDataReader, 2)
                                .Telefono = getStringParameter(theSqlDataReader, 3)
                                .FAX = getStringParameter(theSqlDataReader, 4)
                                .Indirizzo = getStringParameter(theSqlDataReader, 5)
                                .Citta = getStringParameter(theSqlDataReader, 6)
                                .CAP = getStringParameter(theSqlDataReader, 7)
                                .Provincia = getStringParameter(theSqlDataReader, 8)
                                .Blocca_Accesso = getIntegerParameter(theSqlDataReader, 9)
                                .GetNomePC = getStringParameter(theSqlDataReader, 10)
                                .Blocco_Dalle = getDataTimeParameter(theSqlDataReader, 11)
                            End With

                            myArray.Add(myDitta)
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

        Public Function getDitte(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements DitteDAO.getDitte
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myDitta As DitteEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_Ditte"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myDitta = New DitteEntity
                            With myDitta
                                .Codice = theSqlDataReader.GetString(0)
                                .Descrizione = theSqlDataReader.GetString(1)
                                .PartitaIVA = theSqlDataReader.GetString(2)
                                .Telefono = theSqlDataReader.GetString(3)
                                .FAX = theSqlDataReader.GetString(4)
                                .Indirizzo = theSqlDataReader.GetString(5)
                                .Citta = theSqlDataReader.GetString(6)
                                .CAP = theSqlDataReader.GetString(7)
                                .Provincia = theSqlDataReader.GetString(8)
                                .Blocca_Accesso = theSqlDataReader.GetInt32(9)
                                .GetNomePC = theSqlDataReader.GetString(10)
                                .Blocco_Dalle = theSqlDataReader.GetDateTime(11)
                            End With

                            myArray.Add(myDitta)
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