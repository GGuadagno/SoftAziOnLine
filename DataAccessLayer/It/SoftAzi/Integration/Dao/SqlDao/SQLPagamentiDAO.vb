Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLPagamentiDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.PagamentiDAO

        Public Function getPagamenti(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements PagamentiDAO.getPagamenti
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myPagamenti As PagamentiEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_ListaPagamenti"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myPagamenti = New PagamentiEntity
                            With myPagamenti
                                .Codice = getIntegerParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .Tipo_Pagamento = getByteParameter(theSqlDataReader, 2)
                                .Tipo_Scadenza = getByteParameter(theSqlDataReader, 3)
                                .Numero_Rate = getByteParameter(theSqlDataReader, 4)
                                .Mese = getByteParameter(theSqlDataReader, 5)
                                .Scadenza_1 = getIntegerParameter(theSqlDataReader, 6, 16)
                                .Scadenza_2 = getIntegerParameter(theSqlDataReader, 7, 16)
                                .Scadenza_3 = getIntegerParameter(theSqlDataReader, 8, 16)
                                .Scadenza_4 = getIntegerParameter(theSqlDataReader, 9, 16)
                                .Scadenza_5 = getIntegerParameter(theSqlDataReader, 10, 16)
                                .Perc_Imponib_1 = GetFloatParameter(theSqlDataReader, 11)
                                .Perc_Imponib_2 = GetFloatParameter(theSqlDataReader, 12)
                                .Perc_Imponib_3 = GetFloatParameter(theSqlDataReader, 13)
                                .Perc_Imponib_4 = GetFloatParameter(theSqlDataReader, 14)
                                .Perc_Imponib_5 = GetFloatParameter(theSqlDataReader, 15)
                                .Perc_Imposta_1 = GetFloatParameter(theSqlDataReader, 16)
                                .Perc_Imposta_2 = GetFloatParameter(theSqlDataReader, 17)
                                .Perc_Imposta_3 = GetFloatParameter(theSqlDataReader, 18)
                                .Perc_Imposta_4 = GetFloatParameter(theSqlDataReader, 19)
                                .Perc_Imposta_5 = GetFloatParameter(theSqlDataReader, 20)
                                .Mese_Escluso_1 = getByteParameter(theSqlDataReader, 21)
                                .Mese_Escluso_2 = getByteParameter(theSqlDataReader, 22)
                                .Spese_Incasso = getDecimalParameter(theSqlDataReader, 23)
                                .IVA_Spese_Incasso = getIntegerParameter(theSqlDataReader, 24)
                                .Sconto_Cassa = GetFloatParameter(theSqlDataReader, 25)
                            End With

                            myArray.Add(myPagamenti)
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
