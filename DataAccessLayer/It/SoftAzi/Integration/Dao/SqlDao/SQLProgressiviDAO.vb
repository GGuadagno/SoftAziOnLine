Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLProgressiviDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.ProgressiviDAO

        Public Function getProgressivi(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements ProgressiviDAO.getProgressivi
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myProgressivi As ProgressiviEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_Progressivi"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myProgressivi = New ProgressiviEntity
                            With myProgressivi
                                .Effetti = getIntegerParameter(theSqlDataReader, 0, 32)
                                .Distinte = getIntegerParameter(theSqlDataReader, 1, 32)
                                .Ragg_Bil_Clienti = getIntegerParameter(theSqlDataReader, 2, 32)
                                .Ragg_Bil_Fornit = getIntegerParameter(theSqlDataReader, 3, 32)
                                .Bilancio_Apertura = getStringParameter(theSqlDataReader, 4)
                                .Raggr_Clienti = getStringParameter(theSqlDataReader, 5)
                                .Raggr_Fornitori = getStringParameter(theSqlDataReader, 6)
                                .Data_Apertura = getDataTimeParameter(theSqlDataReader, 7)
                                .Chiusura_Patrimoniale = getStringParameter(theSqlDataReader, 8)
                                .Chiusura_Economico = getStringParameter(theSqlDataReader, 9)
                                .Data_Chiusura = getDataTimeParameter(theSqlDataReader, 10)
                                .Utile_Esercizio = getStringParameter(theSqlDataReader, 11)
                                .Perdita_Esercizio = getStringParameter(theSqlDataReader, 12)
                                .Ditta = getStringParameter(theSqlDataReader, 13)
                                .IC_Num = getIntegerParameter(theSqlDataReader, 14, 16)
                                .IC_Sez = getIntegerParameter(theSqlDataReader, 15, 16)
                                .IC_Rinum = getIntegerParameter(theSqlDataReader, 16, 16)
                                .Valuta_1 = getStringParameter(theSqlDataReader, 17)
                                .Valuta_2 = getStringParameter(theSqlDataReader, 18)
                                .Valuta_Dec_1 = getIntegerParameter(theSqlDataReader, 19, 16)
                                .Valuta_Dec_2 = getIntegerParameter(theSqlDataReader, 20, 16)
                                .Cambio_Fisso_2 = getIntegerParameter(theSqlDataReader, 21, 16)
                                .Valuta_Mod_1 = getIntegerParameter(theSqlDataReader, 22, 16)
                                .Valuta_Mod_2 = getIntegerParameter(theSqlDataReader, 23, 16)
                                .Valuta_Old_1 = getStringParameter(theSqlDataReader, 24)
                                .Valuta_Old_2 = getStringParameter(theSqlDataReader, 25)
                                .Perdita_Cambi = getStringParameter(theSqlDataReader, 26)
                                .Utile_Cambi = getStringParameter(theSqlDataReader, 27)
                                .TestoECC = getStringParameter(theSqlDataReader, 28)
                                .PiedeECC = getStringParameter(theSqlDataReader, 29)
                                .Campi_TDL = getIntegerParameter(theSqlDataReader, 30, 16)
                                .ProRata_P = getIntegerParameter(theSqlDataReader, 31, 16)
                                .ProRata_D = getIntegerParameter(theSqlDataReader, 32, 16)
                                .ProRata = getIntegerParameter(theSqlDataReader, 33, 16)
                                .Ragg_Bil_Fornit_RA = getIntegerParameter(theSqlDataReader, 34)
                                .IVA_Corrispettivi = getIntegerParameter(theSqlDataReader, 35, 16)
                                .CentriDiCosto = getIntegerParameter(theSqlDataReader, 36, 16)
                                .IVA_Autotrasporti = getStringParameter(theSqlDataReader, 37)
                                .Ricavi_Autotrasporti = getStringParameter(theSqlDataReader, 38)
                                .Aliquota_Autotrasporti = getIntegerParameter(theSqlDataReader, 39)
                                .Data_Compattazione = getDataTimeParameter(theSqlDataReader, 40)
                                .Dimensione = getIntegerParameter(theSqlDataReader, 41)
                                .PrimaNotaInFattura = getIntegerParameter(theSqlDataReader, 42, 16)
                                .NoteVariazione = getIntegerParameter(theSqlDataReader, 43, 16)
                                .StampaProtCogeInRegIVA = getIntegerParameter(theSqlDataReader, 44, 16)
                                .Reg1 = getStringParameter(theSqlDataReader, 45)
                                .Reg2 = getStringParameter(theSqlDataReader, 46)
                                .TitoloRespBilancio = getStringParameter(theSqlDataReader, 47)
                                .PersonaRespBilancio = getStringParameter(theSqlDataReader, 48)
                                .CapitaleSociale = getStringParameter(theSqlDataReader, 49)
                                .CodAziendaRB = getStringParameter(theSqlDataReader, 50)
                                .UltimaPagRiepIVA = getIntegerParameter(theSqlDataReader, 51)
                                .BloccoCEE = getIntegerParameter(theSqlDataReader, 52, 16)
                                .CentriDiRicavo = getIntegerParameter(theSqlDataReader, 53, 16)
                                .TipoCO = getIntegerParameter(theSqlDataReader, 54, 16)
                                .IFFE = getIntegerParameter(theSqlDataReader, 55, 16)
                                .RegCdCR = getIntegerParameter(theSqlDataReader, 56, 16)
                                .SWMaggRIVA = getIntegerParameter(theSqlDataReader, 57, 16)
                                .PercMaggRIVA = getIntegerParameter(theSqlDataReader, 58)
                                .RACCosto = getStringParameter(theSqlDataReader, 59)
                                .RACCostoCP = getStringParameter(theSqlDataReader, 60)
                                .RACErarioCRit = getStringParameter(theSqlDataReader, 61)
                                .RAGCErarioCRit = getStringParameter(theSqlDataReader, 62)
                                .RAPerc = getIntegerParameter(theSqlDataReader, 63)
                                .RAIVA = getIntegerParameter(theSqlDataReader, 64)
                                .RACausFT = getIntegerParameter(theSqlDataReader, 65)
                                .RACausRA = getIntegerParameter(theSqlDataReader, 66)
                                .CodTributo = getIntegerParameter(theSqlDataReader, 67)
                                .RAPercImp = getIntegerParameter(theSqlDataReader, 68)
                                .RAPercEnasarco = getIntegerParameter(theSqlDataReader, 69)
                                .RAPercCP = getIntegerParameter(theSqlDataReader, 70)
                                .CEnasarco = getStringParameter(theSqlDataReader, 71)
                                .IntraIVACredito = getStringParameter(theSqlDataReader, 72)
                                .IntraIVADebito = getStringParameter(theSqlDataReader, 73)
                                .IntraCosto = getStringParameter(theSqlDataReader, 74)
                                .IntraRicavo = getStringParameter(theSqlDataReader, 75)
                                .SWRagSoc25 = getIntegerParameter(theSqlDataReader, 76, 16)
                                .SezCO = getBooleanParameter(theSqlDataReader, 77)
                                .TestoECC_RiBa = getStringParameter(theSqlDataReader, 78)
                                .PiedeECC_RiBa = getStringParameter(theSqlDataReader, 79)
                            End With

                            myArray.Add(myProgressivi)
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
