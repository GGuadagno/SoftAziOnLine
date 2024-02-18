Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLClientiDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.ClientiDAO

        Public Function getClienti(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements ClientiDAO.getClienti
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myClienti As ClientiEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_Clienti"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myClienti = New ClientiEntity
                            With myClienti
                                .Codice_CoGe = getStringParameter(theSqlDataReader, 0)
                                .Rag_Soc = getStringParameter(theSqlDataReader, 1)
                                .Indirizzo = getStringParameter(theSqlDataReader, 2)
                                .Localita = getStringParameter(theSqlDataReader, 3)
                                .CAP = getStringParameter(theSqlDataReader, 4)
                                .Provincia = getStringParameter(theSqlDataReader, 5)
                                .Nazione = getStringParameter(theSqlDataReader, 6)
                                .Telefono1 = getStringParameter(theSqlDataReader, 7)
                                .Telefono2 = getStringParameter(theSqlDataReader, 8)
                                .Fax = getStringParameter(theSqlDataReader, 9)
                                .Societa = getIntegerParameter(theSqlDataReader, 10, 16)
                                .Codice_Fiscale = getStringParameter(theSqlDataReader, 11)
                                .Partita_IVA = getStringParameter(theSqlDataReader, 12)
                                .Zona = getIntegerParameter(theSqlDataReader, 13, 32)
                                .Categoria = getIntegerParameter(theSqlDataReader, 14, 32)
                                .ABI_N = getStringParameter(theSqlDataReader, 15)
                                .CAB_N = getStringParameter(theSqlDataReader, 16)
                                .Regime_IVA = getIntegerParameter(theSqlDataReader, 17, 32)
                                .Agente_N = getIntegerParameter(theSqlDataReader, 18, 32)
                                .Vettore_N = getIntegerParameter(theSqlDataReader, 19, 32)
                                .Pagamento_N = getIntegerParameter(theSqlDataReader, 20, 32)
                                .Ragg_P = getIntegerParameter(theSqlDataReader, 21, 32)
                                .Bilancio_SN = getStringParameter(theSqlDataReader, 22)
                                .Allegato_IVA = getIntegerParameter(theSqlDataReader, 23, 16)
                                .Apertura = getDecimalParameter(theSqlDataReader, 24)
                                .DA_Apertura = getStringParameter(theSqlDataReader, 25)
                                .Saldo_Dare = getDecimalParameter(theSqlDataReader, 26)
                                .Saldo_Avere = getDecimalParameter(theSqlDataReader, 27)
                                .Data_Agg_Saldi = getDataTimeParameter(theSqlDataReader, 28)
                                .Saldo_Prec = getDecimalParameter(theSqlDataReader, 29)
                                .DA_Saldo_Prec = getStringParameter(theSqlDataReader, 30)
                                .Dare_Chiusura = getDecimalParameter(theSqlDataReader, 31)
                                .Avere_Chiusura = getDecimalParameter(theSqlDataReader, 32)
                                .Riferimento = getStringParameter(theSqlDataReader, 33)
                                .Denominazione = getStringParameter(theSqlDataReader, 34)
                                .Titolare = getStringParameter(theSqlDataReader, 35)
                                .Email = getStringParameter(theSqlDataReader, 36)
                                .Saldo_Dare_2 = getDecimalParameter(theSqlDataReader, 37)
                                .Saldo_Avere_2 = getDecimalParameter(theSqlDataReader, 38)
                                .Dare_Chiusura_2 = getDecimalParameter(theSqlDataReader, 39)
                                .Avere_Chiusura_2 = getDecimalParameter(theSqlDataReader, 40)
                                .Apertura_2 = getDecimalParameter(theSqlDataReader, 41)
                                .DA_Apertura_2 = getStringParameter(theSqlDataReader, 42)
                                .DEM = getIntegerParameter(theSqlDataReader, 43, 16)
                                .Contenitori = getIntegerParameter(theSqlDataReader, 44, 16)
                                .Inviato = getIntegerParameter(theSqlDataReader, 45, 16)
                                .Listino = getIntegerParameter(theSqlDataReader, 46, 32)
                                .Conto_Corrente = getStringParameter(theSqlDataReader, 47)
                                .Credito_1 = getDecimalParameter(theSqlDataReader, 48)
                                .Credito_2 = getDecimalParameter(theSqlDataReader, 49)
                                .Provincia_Estera = getStringParameter(theSqlDataReader, 50)
                                .IndirizzoSenzaNumero = getStringParameter(theSqlDataReader, 51)
                                .NumeroCivico = getStringParameter(theSqlDataReader, 52)
                                .GiornoChiusura_1 = getIntegerParameter(theSqlDataReader, 53, 16)
                                .GiornoChiusura_2 = getIntegerParameter(theSqlDataReader, 54, 16)
                                .ChiusuraMattino_1 = getIntegerParameter(theSqlDataReader, 55, 16)
                                .ChiusuraPomeriggio_1 = getIntegerParameter(theSqlDataReader, 56, 16)
                                .ChiusuraMattino_2 = getIntegerParameter(theSqlDataReader, 57, 16)
                                .ChiusuraPomeriggio_2 = getIntegerParameter(theSqlDataReader, 58, 16)
                                .Note = getStringParameter(theSqlDataReader, 59)
                                .CodProvinciaAgente = getStringParameter(theSqlDataReader, 60)
                                .Agente_N_Prec = getIntegerParameter(theSqlDataReader, 61, 32)
                                .Codice_SEDE = getStringParameter(theSqlDataReader, 62)
                                .Codice_Ricavo = getStringParameter(theSqlDataReader, 63)
                                .IVASosp = getIntegerParameter(theSqlDataReader, 64, 16)
                                .NoFatt = getIntegerParameter(theSqlDataReader, 65, 16)
                                .CIN = getStringParameter(theSqlDataReader, 66)
                                .Data_Nascita = getDataTimeParameter(theSqlDataReader, 67)
                                .NazIBAN = getStringParameter(theSqlDataReader, 68)
                                .CINEUIBAN = getStringParameter(theSqlDataReader, 69)
                                .SWIFT = getStringParameter(theSqlDataReader, 70)
                                .CSAggrAllIVA = getBooleanParameter(theSqlDataReader, 71)
                                .IBAN_Ditta = getStringParameter(theSqlDataReader, 72)
                                .TipoFatt = getStringParameter(theSqlDataReader, 73)
                                .CodiceMABELL = getStringParameter(theSqlDataReader, 74)
                                .IPA = getStringParameter(theSqlDataReader, 75)
                                .SplitIVA = getBooleanParameter(theSqlDataReader, 76)
                                .EmailInvioScad = getStringParameter(theSqlDataReader, 77)
                                .InvioMailScad = getBooleanParameter(theSqlDataReader, 78)
                                .EmailInvioFatt = getStringParameter(theSqlDataReader, 79)
                                .PECEmail = getStringParameter(theSqlDataReader, 80)
                            End With

                            myArray.Add(myClienti)
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
        Public Function getClientiByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As System.Collections.ArrayList Implements ClientiDAO.getClientiByCodice
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myClienti As ClientiEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_ClienteByCodice"
                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.NVarChar, 20)
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
                            myClienti = New ClientiEntity
                            With myClienti
                                .Codice_CoGe = theSqlDataReader.GetString(0)
                                .Rag_Soc = getStringParameter(theSqlDataReader, 1)
                                .Indirizzo = getStringParameter(theSqlDataReader, 2)
                                .Localita = getStringParameter(theSqlDataReader, 3)
                                .CAP = getStringParameter(theSqlDataReader, 4)
                                .Provincia = getStringParameter(theSqlDataReader, 5)
                                .Nazione = getStringParameter(theSqlDataReader, 6)
                                .Telefono1 = getStringParameter(theSqlDataReader, 7)
                                .Telefono2 = getStringParameter(theSqlDataReader, 8)
                                .Fax = getStringParameter(theSqlDataReader, 9)
                                .Societa = getIntegerParameter(theSqlDataReader, 10, 16)
                                .Codice_Fiscale = getStringParameter(theSqlDataReader, 11)
                                .Partita_IVA = getStringParameter(theSqlDataReader, 12)
                                .Zona = getIntegerParameter(theSqlDataReader, 13, 32)
                                .Categoria = getIntegerParameter(theSqlDataReader, 14, 32)
                                .ABI_N = getStringParameter(theSqlDataReader, 15)
                                .CAB_N = getStringParameter(theSqlDataReader, 16)
                                .Regime_IVA = getIntegerParameter(theSqlDataReader, 17, 32)
                                .Agente_N = getIntegerParameter(theSqlDataReader, 18, 32)
                                .Vettore_N = getIntegerParameter(theSqlDataReader, 19, 32)
                                .Pagamento_N = getIntegerParameter(theSqlDataReader, 20, 32)
                                .Ragg_P = getIntegerParameter(theSqlDataReader, 21, 32)
                                .Bilancio_SN = getStringParameter(theSqlDataReader, 22)
                                .Allegato_IVA = getIntegerParameter(theSqlDataReader, 23, 16)
                                .Apertura = getDecimalParameter(theSqlDataReader, 24)
                                .DA_Apertura = getStringParameter(theSqlDataReader, 25)
                                .Saldo_Dare = getDecimalParameter(theSqlDataReader, 26)
                                .Saldo_Avere = getDecimalParameter(theSqlDataReader, 27)
                                .Data_Agg_Saldi = getDataTimeParameter(theSqlDataReader, 28)
                                .Saldo_Prec = getDecimalParameter(theSqlDataReader, 29)
                                .DA_Saldo_Prec = getStringParameter(theSqlDataReader, 30)
                                .Dare_Chiusura = getDecimalParameter(theSqlDataReader, 31)
                                .Avere_Chiusura = getDecimalParameter(theSqlDataReader, 32)
                                .Riferimento = getStringParameter(theSqlDataReader, 33)
                                .Denominazione = getStringParameter(theSqlDataReader, 34)
                                .Titolare = getStringParameter(theSqlDataReader, 35)
                                .Email = getStringParameter(theSqlDataReader, 36)
                                .Saldo_Dare_2 = getDecimalParameter(theSqlDataReader, 37)
                                .Saldo_Avere_2 = getDecimalParameter(theSqlDataReader, 38)
                                .Dare_Chiusura_2 = getDecimalParameter(theSqlDataReader, 39)
                                .Avere_Chiusura_2 = getDecimalParameter(theSqlDataReader, 40)
                                .Apertura_2 = getDecimalParameter(theSqlDataReader, 41)
                                .DA_Apertura_2 = getStringParameter(theSqlDataReader, 42)
                                .DEM = getIntegerParameter(theSqlDataReader, 43, 16)
                                .Contenitori = getIntegerParameter(theSqlDataReader, 44, 16)
                                .Inviato = getIntegerParameter(theSqlDataReader, 45, 16)
                                .Listino = getIntegerParameter(theSqlDataReader, 46, 32)
                                .Conto_Corrente = getStringParameter(theSqlDataReader, 47)
                                .Credito_1 = getDecimalParameter(theSqlDataReader, 48)
                                .Credito_2 = getDecimalParameter(theSqlDataReader, 49)
                                .Provincia_Estera = getStringParameter(theSqlDataReader, 50)
                                .IndirizzoSenzaNumero = getStringParameter(theSqlDataReader, 51)
                                .NumeroCivico = getStringParameter(theSqlDataReader, 52)
                                .GiornoChiusura_1 = getIntegerParameter(theSqlDataReader, 53, 16)
                                .GiornoChiusura_2 = getIntegerParameter(theSqlDataReader, 54, 16)
                                .ChiusuraMattino_1 = getIntegerParameter(theSqlDataReader, 55, 16)
                                .ChiusuraPomeriggio_1 = getIntegerParameter(theSqlDataReader, 56, 16)
                                .ChiusuraMattino_2 = getIntegerParameter(theSqlDataReader, 57, 16)
                                .ChiusuraPomeriggio_2 = getIntegerParameter(theSqlDataReader, 58, 16)
                                .Note = getStringParameter(theSqlDataReader, 59)
                                .CodProvinciaAgente = getStringParameter(theSqlDataReader, 60)
                                .Agente_N_Prec = getIntegerParameter(theSqlDataReader, 61, 32)
                                .Codice_SEDE = getStringParameter(theSqlDataReader, 62)
                                .Codice_Ricavo = getStringParameter(theSqlDataReader, 63)
                                .IVASosp = getIntegerParameter(theSqlDataReader, 64, 16)
                                .NoFatt = getIntegerParameter(theSqlDataReader, 65, 16)
                                .CIN = getStringParameter(theSqlDataReader, 66)
                                .Data_Nascita = getDataTimeParameter(theSqlDataReader, 67)
                                .NazIBAN = getStringParameter(theSqlDataReader, 68)
                                .CINEUIBAN = getStringParameter(theSqlDataReader, 69)
                                .SWIFT = getStringParameter(theSqlDataReader, 70)
                                .CSAggrAllIVA = getBooleanParameter(theSqlDataReader, 71)
                                .IBAN_Ditta = getStringParameter(theSqlDataReader, 72)
                                .TipoFatt = getStringParameter(theSqlDataReader, 73)
                                .CodiceMABELL = getStringParameter(theSqlDataReader, 74)
                                .IPA = getStringParameter(theSqlDataReader, 75)
                                .SplitIVA = getBooleanParameter(theSqlDataReader, 76)
                                .EmailInvioScad = getStringParameter(theSqlDataReader, 77)
                                .InvioMailScad = getBooleanParameter(theSqlDataReader, 78)
                                .EmailInvioFatt = getStringParameter(theSqlDataReader, 79)
                                .PECEmail = getStringParameter(theSqlDataReader, 80)
                            End With

                            myArray.Add(myClienti)
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
        Public Function InsertUpdateCliente(ByVal idTransazione As Integer, ByVal paramClienti As Object) As Boolean Implements ClientiDAO.InsertUpdateCliente
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim theError As Integer
            Dim bReturnOk As Boolean
            Dim myClientiEntity As ClientiEntity = CType(paramClienti, ClientiEntity)

            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "InsertUpdate_Cliente"
                param = setParameter(param, theSqlCommand, "@Codice_CoGe", SqlDbType.NVarChar, myClientiEntity.Codice_CoGe, 16)
                param = setParameter(param, theSqlCommand, "@Rag_Soc", SqlDbType.NVarChar, myClientiEntity.Rag_Soc, 50)
                param = setParameter(param, theSqlCommand, "@Indirizzo", SqlDbType.NVarChar, myClientiEntity.Indirizzo, 50)
                param = setParameter(param, theSqlCommand, "@Localita", SqlDbType.NVarChar, myClientiEntity.Localita, 50)
                param = setParameter(param, theSqlCommand, "@CAP", SqlDbType.NVarChar, myClientiEntity.CAP, 5)
                param = setParameter(param, theSqlCommand, "@Provincia", SqlDbType.NVarChar, myClientiEntity.Provincia, 2)
                param = setParameter(param, theSqlCommand, "@Nazione", SqlDbType.NVarChar, myClientiEntity.Nazione, 3)
                param = setParameter(param, theSqlCommand, "@Telefono1", SqlDbType.NVarChar, myClientiEntity.Telefono1, 30)
                param = setParameter(param, theSqlCommand, "@Telefono2", SqlDbType.NVarChar, myClientiEntity.Telefono2, 30)
                param = setParameter(param, theSqlCommand, "@Fax", SqlDbType.NVarChar, myClientiEntity.Fax, 30)
                param = setParameter(param, theSqlCommand, "@Societa", SqlDbType.Int, myClientiEntity.Societa)
                param = setParameter(param, theSqlCommand, "@Codice_Fiscale", SqlDbType.NVarChar, myClientiEntity.Codice_Fiscale, 16)
                param = setParameter(param, theSqlCommand, "@Partita_IVA", SqlDbType.NVarChar, myClientiEntity.Partita_IVA, 20) 'giu160320
                param = setParameter(param, theSqlCommand, "@Zona", SqlDbType.Int, myClientiEntity.Zona)
                param = setParameter(param, theSqlCommand, "@Categoria", SqlDbType.Int, myClientiEntity.Categoria)
                param = setParameter(param, theSqlCommand, "@ABI_N", SqlDbType.NVarChar, myClientiEntity.ABI_N, 5)
                param = setParameter(param, theSqlCommand, "@CAB_N", SqlDbType.NVarChar, myClientiEntity.CAB_N, 5)
                param = setParameter(param, theSqlCommand, "@Regime_IVA", SqlDbType.Int, myClientiEntity.Regime_IVA)
                param = setParameter(param, theSqlCommand, "@Agente_N", SqlDbType.Int, myClientiEntity.Agente_N)
                param = setParameter(param, theSqlCommand, "@Vettore_N", SqlDbType.Int, myClientiEntity.Vettore_N)
                param = setParameter(param, theSqlCommand, "@Pagamento_N", SqlDbType.Int, myClientiEntity.Pagamento_N)
                param = setParameter(param, theSqlCommand, "@Ragg_P", SqlDbType.Int, myClientiEntity.Ragg_P)
                param = setParameter(param, theSqlCommand, "@Bilancio_SN", SqlDbType.NVarChar, myClientiEntity.Bilancio_SN, 1)
                param = setParameter(param, theSqlCommand, "@Allegato_IVA", SqlDbType.Int, myClientiEntity.Allegato_IVA)
                param = setParameter(param, theSqlCommand, "@Apertura", SqlDbType.Decimal, myClientiEntity.Apertura, 10)
                param = setParameter(param, theSqlCommand, "@DA_Apertura", SqlDbType.NVarChar, myClientiEntity.DA_Apertura, 1)
                param = setParameter(param, theSqlCommand, "@Saldo_Dare", SqlDbType.Decimal, myClientiEntity.Saldo_Dare, 10)
                param = setParameter(param, theSqlCommand, "@Saldo_Avere", SqlDbType.Decimal, myClientiEntity.Saldo_Avere, 10)
                param = setParameter(param, theSqlCommand, "@Data_Agg_Saldi", SqlDbType.DateTime, myClientiEntity.Data_Agg_Saldi, 10)
                param = setParameter(param, theSqlCommand, "@Saldo_Prec", SqlDbType.Decimal, myClientiEntity.Saldo_Prec, 10)
                param = setParameter(param, theSqlCommand, "@DA_Saldo_Prec", SqlDbType.NVarChar, myClientiEntity.DA_Saldo_Prec, 1)
                param = setParameter(param, theSqlCommand, "@Dare_Chiusura", SqlDbType.Decimal, myClientiEntity.Dare_Chiusura, 10)
                param = setParameter(param, theSqlCommand, "@Avere_Chiusura", SqlDbType.Decimal, myClientiEntity.Avere_Chiusura, 10)
                param = setParameter(param, theSqlCommand, "@Riferimento", SqlDbType.NVarChar, myClientiEntity.Riferimento, 500) 'giu310123
                param = setParameter(param, theSqlCommand, "@Denominazione", SqlDbType.NVarChar, myClientiEntity.Denominazione, 50)
                param = setParameter(param, theSqlCommand, "@Titolare", SqlDbType.NVarChar, myClientiEntity.Titolare, 50)
                param = setParameter(param, theSqlCommand, "@Email", SqlDbType.NVarChar, myClientiEntity.Email, 100)
                param = setParameter(param, theSqlCommand, "@Saldo_Dare_2", SqlDbType.Decimal, myClientiEntity.Saldo_Dare_2, 10)
                param = setParameter(param, theSqlCommand, "@Saldo_Avere_2", SqlDbType.Decimal, myClientiEntity.Saldo_Avere_2, 10)
                param = setParameter(param, theSqlCommand, "@Dare_Chiusura_2", SqlDbType.Decimal, myClientiEntity.Dare_Chiusura_2, 10)
                param = setParameter(param, theSqlCommand, "@Avere_Chiusura_2", SqlDbType.Decimal, myClientiEntity.Avere_Chiusura_2, 10)
                param = setParameter(param, theSqlCommand, "@Apertura_2", SqlDbType.Decimal, myClientiEntity.Apertura_2, 10)
                param = setParameter(param, theSqlCommand, "@DA_Apertura_2", SqlDbType.NVarChar, myClientiEntity.DA_Apertura_2, 1)
                param = setParameter(param, theSqlCommand, "@DEM", SqlDbType.Int, myClientiEntity.DEM, 4)
                param = setParameter(param, theSqlCommand, "@Contenitori", SqlDbType.Int, myClientiEntity.Contenitori, 4)
                param = setParameter(param, theSqlCommand, "@Inviato", SqlDbType.Int, myClientiEntity.Inviato, 4)
                param = setParameter(param, theSqlCommand, "@Listino", SqlDbType.Int, myClientiEntity.Listino, 4)
                param = setParameter(param, theSqlCommand, "@Conto_Corrente", SqlDbType.NVarChar, myClientiEntity.Conto_Corrente, 15)
                param = setParameter(param, theSqlCommand, "@Credito_1", SqlDbType.Decimal, myClientiEntity.Credito_1, 10)
                param = setParameter(param, theSqlCommand, "@Credito_2", SqlDbType.Decimal, myClientiEntity.Credito_2, 10)
                param = setParameter(param, theSqlCommand, "@Provincia_Estera", SqlDbType.NVarChar, myClientiEntity.Provincia_Estera, 15)
                param = setParameter(param, theSqlCommand, "@IndirizzoSenzaNumero", SqlDbType.NVarChar, myClientiEntity.IndirizzoSenzaNumero, 50)
                param = setParameter(param, theSqlCommand, "@NumeroCivico", SqlDbType.NVarChar, myClientiEntity.NumeroCivico, 10)
                param = setParameter(param, theSqlCommand, "@GiornoChiusura_1", SqlDbType.Int, myClientiEntity.GiornoChiusura_1, 4)
                param = setParameter(param, theSqlCommand, "@GiornoChiusura_2", SqlDbType.Int, myClientiEntity.GiornoChiusura_2, 4)
                param = setParameter(param, theSqlCommand, "@ChiusuraMattino_1", SqlDbType.Int, myClientiEntity.ChiusuraMattino_1, 4)
                param = setParameter(param, theSqlCommand, "@ChiusuraPomeriggio_1", SqlDbType.Int, myClientiEntity.ChiusuraPomeriggio_1, 4)
                param = setParameter(param, theSqlCommand, "@ChiusuraMattino_2", SqlDbType.Int, myClientiEntity.ChiusuraMattino_2, 4)
                param = setParameter(param, theSqlCommand, "@ChiusuraPomeriggio_2", SqlDbType.Int, myClientiEntity.ChiusuraPomeriggio_2, 4)
                param = setParameter(param, theSqlCommand, "@Note", SqlDbType.NText, myClientiEntity.Note)
                param = setParameter(param, theSqlCommand, "@CodProvinciaAgente", SqlDbType.NVarChar, myClientiEntity.CodProvinciaAgente, 5)
                param = setParameter(param, theSqlCommand, "@Agente_N_Prec", SqlDbType.Int, myClientiEntity.Agente_N_Prec, 4)
                param = setParameter(param, theSqlCommand, "@Codice_SEDE", SqlDbType.NVarChar, myClientiEntity.Codice_SEDE, 16)
                param = setParameter(param, theSqlCommand, "@Codice_Ricavo", SqlDbType.NVarChar, myClientiEntity.Codice_Ricavo, 16)
                param = setParameter(param, theSqlCommand, "@IVASosp", SqlDbType.Int, myClientiEntity.IVASosp, 4)
                param = setParameter(param, theSqlCommand, "@NoFatt", SqlDbType.Int, myClientiEntity.NoFatt, 4)
                param = setParameter(param, theSqlCommand, "@CIN", SqlDbType.NVarChar, myClientiEntity.CIN, 1)
                param = setParameter(param, theSqlCommand, "@Data_Nascita", SqlDbType.DateTime, myClientiEntity.Data_Nascita, 10)
                param = setParameter(param, theSqlCommand, "@NazIBAN", SqlDbType.NVarChar, myClientiEntity.NazIBAN, 3)
                param = setParameter(param, theSqlCommand, "@CINEUIBAN", SqlDbType.NVarChar, myClientiEntity.CINEUIBAN, 2)
                param = setParameter(param, theSqlCommand, "@SWIFT", SqlDbType.NVarChar, myClientiEntity.SWIFT, 15)
                param = setParameter(param, theSqlCommand, "@CSAggrAllIVA", SqlDbType.Bit, myClientiEntity.CSAggrAllIVA, 1)
                param = setParameter(param, theSqlCommand, "@IBAN_Ditta", SqlDbType.NVarChar, myClientiEntity.IBAN_Ditta, 27)
                param = setParameter(param, theSqlCommand, "@InseritoDa", SqlDbType.NVarChar, myClientiEntity.InseritoDa, 50)
                param = setParameter(param, theSqlCommand, "@ModificatoDa", SqlDbType.NVarChar, myClientiEntity.ModificatoDa, 50)
                param = setParameter(param, theSqlCommand, "@TipoFatt", SqlDbType.NVarChar, myClientiEntity.TipoFatt, 2)
                param = setParameter(param, theSqlCommand, "@CodiceMABELL", SqlDbType.NVarChar, myClientiEntity.CodiceMABELL, 16)
                param = setParameter(param, theSqlCommand, "@IPA", SqlDbType.NVarChar, myClientiEntity.IPA, 10)
                param = setParameter(param, theSqlCommand, "@SplitIVA", SqlDbType.Bit, myClientiEntity.SplitIVA, 1)
                param = setParameter(param, theSqlCommand, "@EmailInvioScad", SqlDbType.NVarChar, myClientiEntity.EmailInvioScad, 100)
                param = setParameter(param, theSqlCommand, "@InvioMailScad", SqlDbType.Bit, myClientiEntity.InvioMailScad, 1)
                param = setParameter(param, theSqlCommand, "@EmailInvioFatt", SqlDbType.NVarChar, myClientiEntity.EmailInvioFatt, 100)
                param = setParameter(param, theSqlCommand, "@PECEmail", SqlDbType.NVarChar, myClientiEntity.PECEmail, 310)
                param = theSqlCommand.Parameters.Add("@RetVal", SqlDbType.Int, 4)
                param.Direction = ParameterDirection.Output
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = -1 Then
                        bReturnOk = False
                    Else
                        bReturnOk = True
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
        Public Function delClientiByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements ClientiDAO.delClientiByCodice
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
                theSqlCommand.CommandText = "del_ClienteByCodice"
                param = theSqlCommand.Parameters.Add("@Codice_CoGe", SqlDbType.NVarChar, 16)
                param.Direction = ParameterDirection.Input
                param.Value = Codice
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = -1 Then
                        bReturnOk = False
                    Else
                        bReturnOk = True
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
        Public Function CIClienteByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements ClientiDAO.CIClienteByCodice
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
                theSqlCommand.CommandText = "CI_ClienteByCodice"
                param = theSqlCommand.Parameters.Add("@Codice_CoGe", SqlDbType.NVarChar, 20)
                param.Direction = ParameterDirection.Input
                param.Value = Codice
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
        Public Function CIClienteByCodiceAZI(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements ClientiDAO.CIClienteByCodiceAZI
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
                theSqlCommand.CommandText = "CI_ClienteByCodiceAZI"
                param = theSqlCommand.Parameters.Add("@Codice_CoGe", SqlDbType.NVarChar, 20)
                param.Direction = ParameterDirection.Input
                param.Value = Codice
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
        Public Function CIClienteByCodiceSCAD(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements ClientiDAO.CIClienteByCodiceSCAD
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
                theSqlCommand.CommandText = "CI_ClienteByCodiceSCAD"
                param = theSqlCommand.Parameters.Add("@Codice_CoGe", SqlDbType.NVarChar, 20)
                param.Direction = ParameterDirection.Input
                param.Value = Codice
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
