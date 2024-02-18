Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLPianoDeiContiDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.PianoDeiContiDAO

        Public Function getPianoDeiConti(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements PianoDeiContiDAO.getPianoDeiConti
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myPianoDeiConti As PianoDeiContiEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_PianoDeiConti"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myPianoDeiConti = New PianoDeiContiEntity
                            With myPianoDeiConti
                                .Codice_CoGe = getStringParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .Tipo = getStringParameter(theSqlDataReader, 2)
                                .E_P = getStringParameter(theSqlDataReader, 3)
                                .Bilancio_SN = getStringParameter(theSqlDataReader, 4)
                                .Sezione = getStringParameter(theSqlDataReader, 5)
                                .Apertura = getDecimalParameter(theSqlDataReader, 6)
                                .DA_Apertura = getStringParameter(theSqlDataReader, 7)
                                .Saldo_Dare = getDecimalParameter(theSqlDataReader, 8)
                                .Saldo_Avere = getDecimalParameter(theSqlDataReader, 9)
                                .Data_Agg_Saldi = getDataTimeParameter(theSqlDataReader, 10)
                                .Saldo_Prec = getDecimalParameter(theSqlDataReader, 11)
                                .DA_Saldo_Prec = getStringParameter(theSqlDataReader, 12)
                                .Conto_Banca = getIntegerParameter(theSqlDataReader, 13, 16)
                                .ABI = getStringParameter(theSqlDataReader, 14)
                                .CAB = getStringParameter(theSqlDataReader, 15)
                                .Ragg_E_P = getIntegerParameter(theSqlDataReader, 16)
                                .Livello = getIntegerParameter(theSqlDataReader, 17)
                                .Dare_Chiusura = getDecimalParameter(theSqlDataReader, 18)
                                .Avere_Chiusura = getDecimalParameter(theSqlDataReader, 19)
                                .Saldo_Dare_2 = getDecimalParameter(theSqlDataReader, 20)
                                .Saldo_Avere_2 = getDecimalParameter(theSqlDataReader, 21)
                                .Dare_Chiusura_2 = getDecimalParameter(theSqlDataReader, 22)
                                .Avere_Chiusura_2 = getDecimalParameter(theSqlDataReader, 23)
                                .Apertura_2 = getDecimalParameter(theSqlDataReader, 24)
                                .Entrambe = getIntegerParameter(theSqlDataReader, 25, 16)
                            End With

                            myArray.Add(myPianoDeiConti)
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
