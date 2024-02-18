Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLParametriGeneraliAziDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.ParametriGeneraliAziDAO

        Public Function getParametriGeneraliAzi(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements ParametriGeneraliAziDAO.getParametriGeneraliAzi
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myParamGenAzi As ParametriGeneraliAziEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_ParametriGeneraliAzi"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myParamGenAzi = New ParametriGeneraliAziEntity
                            With myParamGenAzi
                                .RighePerPaginaDDT = getIntegerParameter(theSqlDataReader, 0)
                                .RighePerPaginaFatt = getIntegerParameter(theSqlDataReader, 1)
                                .NumeroDDT = getIntegerParameter(theSqlDataReader, 2)
                                .NumeroFattura = getIntegerParameter(theSqlDataReader, 3)
                                .NumeroNotaAccredito = getIntegerParameter(theSqlDataReader, 4)
                                .NumeroNotaCdenza = getIntegerParameter(theSqlDataReader, 5)
                                .NoteAccreditoNumerazioneSeparata = getBooleanParameter(theSqlDataReader, 6)
                                .NoteCdenzaNumSep = getBooleanParameter(theSqlDataReader, 7)
                                .IVATrasporto = getIntegerParameter(theSqlDataReader, 8)
                                .AnteprimaStampa = getBooleanParameter(theSqlDataReader, 9)
                                .RighePerPaginaORDINI = getIntegerParameter(theSqlDataReader, 10)
                                .CodiceContoRicavoCOGE = getStringParameter(theSqlDataReader, 11)
                                .CodiceCausaleCOGE = getIntegerParameter(theSqlDataReader, 12)
                                .AggiornaMagazzino = getBooleanParameter(theSqlDataReader, 13)
                                .DataUltimaCompattazione = getDataTimeParameter(theSqlDataReader, 14)
                                .NumeroOrdineFornitore = getIntegerParameter(theSqlDataReader, 15)
                                .NumeroRiordinoFornitore = getIntegerParameter(theSqlDataReader, 16)
                                .CodiceCausaleRiordino = getIntegerParameter(theSqlDataReader, 17)
                                .LarghezzaBolla = getIntegerParameter(theSqlDataReader, 18)
                                .MaxDescrizione = getIntegerParameter(theSqlDataReader, 19)
                                .ContoCorrispettivi = getStringParameter(theSqlDataReader, 20)
                                .ContoCassa = getStringParameter(theSqlDataReader, 21)
                                .CodiceCausaleIncasso = getIntegerParameter(theSqlDataReader, 22)
                                .ChiedoListino = getBooleanParameter(theSqlDataReader, 23)
                                .CodiceDescrizione = getBooleanParameter(theSqlDataReader, 24)
                                .DueCopie = getBooleanParameter(theSqlDataReader, 25)
                                .CodiceCausaleTrasferimento = getIntegerParameter(theSqlDataReader, 26)
                                .IvaSpese = getIntegerParameter(theSqlDataReader, 27)
                                .CodiceCausaleCorrisp = getIntegerParameter(theSqlDataReader, 28)
                                .CodiceCausaleCOGENA = getIntegerParameter(theSqlDataReader, 29)
                                .CodiceCausaleIncassoNA = getIntegerParameter(theSqlDataReader, 30)
                                .ContoSpeseIncasso = getStringParameter(theSqlDataReader, 31)
                                .ContoSpeseTrasporto = getStringParameter(theSqlDataReader, 32)
                                .ContoSpeseVarie = getStringParameter(theSqlDataReader, 33)
                                .CodiceCausaleTrasferimentoFiliale = getIntegerParameter(theSqlDataReader, 34)
                                .StringaConai = getStringParameter(theSqlDataReader, 35)
                                .StringaBolla = getStringParameter(theSqlDataReader, 36)
                                .AspettoDeiBeni = getStringParameter(theSqlDataReader, 37)
                                .CalcoloColliAutomatico = getBooleanParameter(theSqlDataReader, 38)
                                .PasswordMovimenti = getStringParameter(theSqlDataReader, 39)
                                .Iva_Imballo = getIntegerParameter(theSqlDataReader, 40)
                                .ContoSpeseImballo = getStringParameter(theSqlDataReader, 41)
                                .DicituraASPEST = getStringParameter(theSqlDataReader, 42)
                                .DicituraPORTO = getStringParameter(theSqlDataReader, 43)
                                .NumeroOrdineCliente = getIntegerParameter(theSqlDataReader, 44)
                                .CarPerRiga = getIntegerParameter(theSqlDataReader, 45)
                                .Decimali_Sconto = getIntegerParameter(theSqlDataReader, 46)
                                .Decimali_Provvigione = getIntegerParameter(theSqlDataReader, 47)
                                .NumeroPreventivo = getIntegerParameter(theSqlDataReader, 48)
                                .RighePerPaginaPrev = getIntegerParameter(theSqlDataReader, 49)
                                .ContoRiBa = getStringParameter(theSqlDataReader, 50)
                                .CausaleRiBa = getIntegerParameter(theSqlDataReader, 51)
                                .RegIncasso = getBooleanParameter(theSqlDataReader, 52)
                                .NumSconti = getIntegerParameter(theSqlDataReader, 53)
                                .Num_Differenziata = getBooleanParameter(theSqlDataReader, 54)
                                .Cod_Valuta = getStringParameter(theSqlDataReader, 55)
                                .Decimali_Prezzi = getIntegerParameter(theSqlDataReader, 56)
                                .Visual_2_Valute = getBooleanParameter(theSqlDataReader, 57)
                                .causaleMMpos = getIntegerParameter(theSqlDataReader, 58)
                                .causaleMMneg = getIntegerParameter(theSqlDataReader, 59)
                                .Decimali_Prezzi_2 = getIntegerParameter(theSqlDataReader, 60)
                                .CodCausaleVendita = getIntegerParameter(theSqlDataReader, 61)
                                .CodiceNumerico = getBooleanParameter(theSqlDataReader, 62)
                                .LunghezzaMaxCodice = getIntegerParameter(theSqlDataReader, 63)
                                .ValoreMinimoOrdine = getDecimalParameter(theSqlDataReader, 64)
                                .gg_lavorativi_sett = getIntegerParameter(theSqlDataReader, 65)
                                .giorno_riposo = getIntegerParameter(theSqlDataReader, 66)
                                .sett_verifica_qta = getIntegerParameter(theSqlDataReader, 67)
                                .NumScontiForn = getIntegerParameter(theSqlDataReader, 68)
                                .DecScontoForn = getIntegerParameter(theSqlDataReader, 69)
                                .ControlloSottoscorta = getBooleanParameter(theSqlDataReader, 70)
                                .NumeroSped = getIntegerParameter(theSqlDataReader, 71)
                                .RegPNRB = getBooleanParameter(theSqlDataReader, 72)
                                .LivelloMaxDistBase = getIntegerParameter(theSqlDataReader, 73)
                                .CalcoloScontoSuImporto = getBooleanParameter(theSqlDataReader, 74)
                                .CodTipoFatt = getStringParameter(theSqlDataReader, 75)
                                .CausaleRipristinoSaldi = getIntegerParameter(theSqlDataReader, 76)
                                .DisabilitaRiordino = getBooleanParameter(theSqlDataReader, 77)
                                .AnniFuoriProd = getIntegerParameter(theSqlDataReader, 78)
                                .NumeroOrdineDaDeposito = getIntegerParameter(theSqlDataReader, 79)
                                .CausSBNatale = getIntegerParameter(theSqlDataReader, 80)
                                .CausSBPasqua = getIntegerParameter(theSqlDataReader, 81)
                                .CausDDTDep = getIntegerParameter(theSqlDataReader, 82)
                                .CausVendDep = getIntegerParameter(theSqlDataReader, 83)
                                .CausResoDep = getIntegerParameter(theSqlDataReader, 84)
                                .PercorsoStampaOrdini = getStringParameter(theSqlDataReader, 85)
                                .PercorsoStampaDDT = getStringParameter(theSqlDataReader, 86)
                                .PercorsoStampaFatt = getStringParameter(theSqlDataReader, 87)
                                .PercorsoStampaPrev = getStringParameter(theSqlDataReader, 88)
                                .CausNCResi = getIntegerParameter(theSqlDataReader, 89)
                                .CausNCAbbuono = getIntegerParameter(theSqlDataReader, 90)
                                .CausNCScontoOmesso = getIntegerParameter(theSqlDataReader, 91)
                                .CausNCDiffPrezzo = getIntegerParameter(theSqlDataReader, 92)
                                .UltAgRicalcolato = getIntegerParameter(theSqlDataReader, 93)
                                .CausNCSBPrec = getIntegerParameter(theSqlDataReader, 94)
                                .NumeroBC = getIntegerParameter(theSqlDataReader, 95)
                                .DueCopieNZ = getBooleanParameter(theSqlDataReader, 96)
                                .CausRimInizialeDep = getIntegerParameter(theSqlDataReader, 97)
                                .ComunicazioneRintracciabilita = getStringParameter(theSqlDataReader, 98)
                                .PieDiPaginaRintracciabilita = getStringParameter(theSqlDataReader, 99)
                                .SMTPServer = getStringParameter(theSqlDataReader, 100)
                                .SMTPPorta = getIntegerParameter(theSqlDataReader, 101)
                                .SMTPUserName = getStringParameter(theSqlDataReader, 102)
                                .SMTPPassword = getStringParameter(theSqlDataReader, 103)
                                .SMTPMailSender = getStringParameter(theSqlDataReader, 104)
                                .DettaglioCestiDDT = getBooleanParameter(theSqlDataReader, 105)
                                .NumeroOCL = getIntegerParameter(theSqlDataReader, 106)
                                .CausOrdCL = getIntegerParameter(theSqlDataReader, 107)
                                .CausDDTCL = getIntegerParameter(theSqlDataReader, 108)
                                .CausResoCL = getIntegerParameter(theSqlDataReader, 109)
                                .CausFineCL = getIntegerParameter(theSqlDataReader, 110)
                                .CausRestiCL = getIntegerParameter(theSqlDataReader, 111)
                                .CausCarMagCL = getIntegerParameter(theSqlDataReader, 112)
                                .ListinoCL = getIntegerParameter(theSqlDataReader, 113)
                                .Banca = getStringParameter(theSqlDataReader, 114)
                                .ABI = getStringParameter(theSqlDataReader, 115)
                                .CAB = getStringParameter(theSqlDataReader, 116)
                                .CIN = getStringParameter(theSqlDataReader, 117)
                                .CC = getStringParameter(theSqlDataReader, 118)
                                .NazIBAN = getStringParameter(theSqlDataReader, 119)
                                .CINEUIBAN = getStringParameter(theSqlDataReader, 120)
                                .SWIFT = getStringParameter(theSqlDataReader, 121)
                                .PrezziDDT = getBooleanParameter(theSqlDataReader, 122)
                                .NUltimiPrezziAcq = getIntegerParameter(theSqlDataReader, 123)
                                .Decimali_Grandezze = getIntegerParameter(theSqlDataReader, 124)
                                .NGG_Validita = getIntegerParameter(theSqlDataReader, 125)
                                .NGG_Consegna = getIntegerParameter(theSqlDataReader, 126)
                                .NumeroFA = getIntegerParameter(theSqlDataReader, 127) 'GIU240312
                                .NumeroPA = getIntegerParameter(theSqlDataReader, 128) 'GIU210714
                                'GIU220714
                                .NumeroNCPA = getIntegerParameter(theSqlDataReader, 129)
                                .NumeroNCPASep = getBooleanParameter(theSqlDataReader, 130)
                                'giu300718
                                .SelAICatCli = getIntegerParameter(theSqlDataReader, 131)
                                .SelAIDaData = getIntegerParameter(theSqlDataReader, 132)
                                .SelAIAData = getIntegerParameter(theSqlDataReader, 133)
                                .AIServizioEmail = getBooleanParameter(theSqlDataReader, 134)
                                .SelAIScGa = getBooleanParameter(theSqlDataReader, 135)
                                .SelAIScEl = getBooleanParameter(theSqlDataReader, 136)
                                .SelAIScBa = getBooleanParameter(theSqlDataReader, 137)
                                .ScCassaDett = getBooleanParameter(theSqlDataReader, 138) 'giu311218
                                'giu270219
                                .ImpMinBollo = getDecimalParameter(theSqlDataReader, 139)
                                .IVABollo = getIntegerParameter(theSqlDataReader, 140)
                                .IVAScMerce = getIntegerParameter(theSqlDataReader, 141)
                                .ContoRitAcconto = getStringParameter(theSqlDataReader, 142)
                                .Bollo = getDecimalParameter(theSqlDataReader, 143)
                                .AIServizioEmailAttiva = getStringParameter(theSqlDataReader, 144)
                            End With
                            myArray.Add(myParamGenAzi)
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
