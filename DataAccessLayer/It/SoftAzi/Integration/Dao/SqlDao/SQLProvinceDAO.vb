Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLProvinceDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.ProvinceDAO

        Public Function getProvince(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements ProvinceDAO.getProvince
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myProvince As ProvinceEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_Province"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myProvince = New ProvinceEntity
                            With myProvince
                                .Codice = getStringParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .Regione = getIntegerParameter(theSqlDataReader, 2)
                            End With

                            myArray.Add(myProvince)
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
