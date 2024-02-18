Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLAliquoteIvaDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.AliquoteIvaDAO

        Public Function getAliquoteIva(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements AliquoteIvaDAO.getAliquoteIva
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myAliquoteIva As AliquoteIvaEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_AliquoteIva"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myAliquoteIva = New AliquoteIvaEntity
                            With myAliquoteIva
                                .Aliquota = getIntegerParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .ProRata = getIntegerParameter(theSqlDataReader, 2)
                                .Escludi_ProRata = getIntegerParameter(theSqlDataReader, 3)
                                .TipOp = getIntegerParameter(theSqlDataReader, 4)
                                .AliqIVA = getIntegerParameter(theSqlDataReader, 5)
                            End With

                            myArray.Add(myAliquoteIva)
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
