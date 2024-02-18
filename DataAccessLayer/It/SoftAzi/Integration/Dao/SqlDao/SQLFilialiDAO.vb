Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil

Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLFilialiDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.FilialiDAO

        Public Function getFilialiElenco(ByVal idTransazione As Integer, ByVal ABI As String) As System.Collections.ArrayList Implements FilialiDAO.getFilialiElenco
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myFiliali As FilialiEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "Get_ElencoFilialiByABI"
                param = theSqlCommand.Parameters.Add("@ABI", SqlDbType.NVarChar, 5)
                param.Direction = ParameterDirection.Input
                param.Value = ABI
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myFiliali = New FilialiEntity
                            With myFiliali
                                .ABI = getStringParameter(theSqlDataReader, 0)
                                .CAB = getStringParameter(theSqlDataReader, 1)
                                .Filiale = getStringParameter(theSqlDataReader, 2)
                                .Nazione = getStringParameter(theSqlDataReader, 3)
                                .CAP = getStringParameter(theSqlDataReader, 4)
                                .Provincia = getStringParameter(theSqlDataReader, 5)
                                .Citta = getStringParameter(theSqlDataReader, 6)
                                .Indirizzo = getStringParameter(theSqlDataReader, 7)
                            End With

                            myArray.Add(myFiliali)
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