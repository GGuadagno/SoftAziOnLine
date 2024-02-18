Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLAgentiDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.AgentiDAO

        Public Function getAgenti(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements AgentiDAO.getAgenti
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myAgenti As AgentiEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_Agenti"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myAgenti = New AgentiEntity
                            With myAgenti
                                .Codice = getIntegerParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .Residenza = getStringParameter(theSqlDataReader, 2)
                                .Localita = getStringParameter(theSqlDataReader, 3)
                                .Provincia = getStringParameter(theSqlDataReader, 4)
                                .Partita_IVA = getStringParameter(theSqlDataReader, 5)
                                .Aliquota_IVA = getIntegerParameter(theSqlDataReader, 6)
                                .Aliquota_RitAcc = getDoubleParameter(theSqlDataReader, 7)
                                .Aliquota_Enasarco = getDoubleParameter(theSqlDataReader, 8)
                                .Codice_CoGe = getStringParameter(theSqlDataReader, 9)
                                .Estero = getIntegerParameter(theSqlDataReader, 10)
                                .DataInizioCollaborazione = getDataTimeParameter(theSqlDataReader, 11)
                                .DataFineCollaborazione = getDataTimeParameter(theSqlDataReader, 12)
                                .CodCapogruppo = getIntegerParameter(theSqlDataReader, 13)
                            End With

                            myArray.Add(myAgenti)
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
