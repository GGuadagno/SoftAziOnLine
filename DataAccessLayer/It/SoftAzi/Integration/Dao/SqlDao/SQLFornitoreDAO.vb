Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLFornitoreDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.FornitoreDAO

        Public Function getFornitori(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements FornitoreDAO.getFornitori
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myFornitori As FornitoreEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_Fornitori"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myFornitori = New FornitoreEntity
                            With myFornitori
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
                                .Pagamento_N = getIntegerParameter(theSqlDataReader, 18, 32)
                                .Bilancio_SN = getStringParameter(theSqlDataReader, 19)
                                .Ragg_P = getIntegerParameter(theSqlDataReader, 20, 32)
                                .Allegato_IVA = getIntegerParameter(theSqlDataReader, 21, 16)
                                .Apertura = getDecimalParameter(theSqlDataReader, 22)
                                .DA_Apertura = getStringParameter(theSqlDataReader, 23)
                                .Saldo_Dare = getDecimalParameter(theSqlDataReader, 24)
                                .Saldo_Avere = getDecimalParameter(theSqlDataReader, 25)
                                .Data_Agg_Saldi = getDataTimeParameter(theSqlDataReader, 26)
                                .Saldo_Prec = getDecimalParameter(theSqlDataReader, 27)
                                .DA_Saldo_Prec = getStringParameter(theSqlDataReader, 28)
                                .Dare_Chiusura = getDecimalParameter(theSqlDataReader, 29)
                                .Avere_Chiusura = getDecimalParameter(theSqlDataReader, 30)
                                .Riferimento = getStringParameter(theSqlDataReader, 31)
                                .Denominazione = getStringParameter(theSqlDataReader, 32)
                                .Titolare = getStringParameter(theSqlDataReader, 33)
                                .Email = getStringParameter(theSqlDataReader, 34)
                                .Saldo_Dare_2 = getDecimalParameter(theSqlDataReader, 35)
                                .Saldo_Avere_2 = getDecimalParameter(theSqlDataReader, 36)
                                .Dare_Chiusura_2 = getDecimalParameter(theSqlDataReader, 37)
                                .Avere_Chiusura_2 = getDecimalParameter(theSqlDataReader, 38)
                                .Apertura_2 = getDecimalParameter(theSqlDataReader, 39)
                                .DA_Apertura_2 = getStringParameter(theSqlDataReader, 40)
                                .FuoriZona = getIntegerParameter(theSqlDataReader, 41, 16)
                                .Modalita_Invio_Ordine = getStringParameter(theSqlDataReader, 42)
                                .Data_Inizio_Chiusura = getDataTimeParameter(theSqlDataReader, 43)
                                .Data_Fine_Chiusura = getDataTimeParameter(theSqlDataReader, 44)
                                .Conto_Corrente = getStringParameter(theSqlDataReader, 45)
                                .Rit_Acconto = getIntegerParameter(theSqlDataReader, 46, 16)
                                .Provincia_Estera = getStringParameter(theSqlDataReader, 47)
                                .IndirizzoSenzaNumero = getStringParameter(theSqlDataReader, 48)
                                .NumeroCivico = getStringParameter(theSqlDataReader, 49)
                                .Data_Nascita = getDataTimeParameter(theSqlDataReader, 50)
                                .StRit_Acconto = getIntegerParameter(theSqlDataReader, 51, 16)
                                .CodTributo = getIntegerParameter(theSqlDataReader, 52, 32)
                                .Codice_SEDE = getStringParameter(theSqlDataReader, 53)
                                .Codice_Costo = getStringParameter(theSqlDataReader, 54)
                                .NoFatt = getIntegerParameter(theSqlDataReader, 55, 16)
                                .CIN = getStringParameter(theSqlDataReader, 56)
                                .NazIBAN = getStringParameter(theSqlDataReader, 57)
                                .CINEUIBAN = getStringParameter(theSqlDataReader, 58)
                                .SWIFT = getStringParameter(theSqlDataReader, 59)
                                .Listino = getIntegerParameter(theSqlDataReader, 60, 32)
                                .CSAggrAllIVA = getBooleanParameter(theSqlDataReader, 61)
                                .IBAN_Ditta = getStringParameter(theSqlDataReader, 62) 'Pier220612
                                .PECEMail = getStringParameter(theSqlDataReader, 63)
                            End With

                            myArray.Add(myFornitori)
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
        Public Function getFornitoreByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As System.Collections.ArrayList Implements FornitoreDAO.getFornitoreByCodice
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myFornitore As FornitoreEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_FornitoreByCodice"
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
                            myFornitore = New FornitoreEntity
                            With myFornitore
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
                                .Pagamento_N = getIntegerParameter(theSqlDataReader, 18, 32)
                                .Bilancio_SN = getStringParameter(theSqlDataReader, 19)
                                .Ragg_P = getIntegerParameter(theSqlDataReader, 20, 32)
                                .Allegato_IVA = getIntegerParameter(theSqlDataReader, 21, 16)
                                .Apertura = getDecimalParameter(theSqlDataReader, 22)
                                .DA_Apertura = getStringParameter(theSqlDataReader, 23)
                                .Saldo_Dare = getDecimalParameter(theSqlDataReader, 24)
                                .Saldo_Avere = getDecimalParameter(theSqlDataReader, 25)
                                .Data_Agg_Saldi = getDataTimeParameter(theSqlDataReader, 26)
                                .Saldo_Prec = getDecimalParameter(theSqlDataReader, 27)
                                .DA_Saldo_Prec = getStringParameter(theSqlDataReader, 28)
                                .Dare_Chiusura = getDecimalParameter(theSqlDataReader, 29)
                                .Avere_Chiusura = getDecimalParameter(theSqlDataReader, 30)
                                .Riferimento = getStringParameter(theSqlDataReader, 31)
                                .Denominazione = getStringParameter(theSqlDataReader, 32)
                                .Titolare = getStringParameter(theSqlDataReader, 33)
                                .Email = getStringParameter(theSqlDataReader, 34)
                                .Saldo_Dare_2 = getDecimalParameter(theSqlDataReader, 35)
                                .Saldo_Avere_2 = getDecimalParameter(theSqlDataReader, 36)
                                .Dare_Chiusura_2 = getDecimalParameter(theSqlDataReader, 37)
                                .Avere_Chiusura_2 = getDecimalParameter(theSqlDataReader, 38)
                                .Apertura_2 = getDecimalParameter(theSqlDataReader, 39)
                                .DA_Apertura_2 = getStringParameter(theSqlDataReader, 40)
                                .FuoriZona = getIntegerParameter(theSqlDataReader, 41, 16)
                                .Modalita_Invio_Ordine = getStringParameter(theSqlDataReader, 42)
                                .Data_Inizio_Chiusura = getDataTimeParameter(theSqlDataReader, 43)
                                .Data_Fine_Chiusura = getDataTimeParameter(theSqlDataReader, 44)
                                .Conto_Corrente = getStringParameter(theSqlDataReader, 45)
                                .Rit_Acconto = getIntegerParameter(theSqlDataReader, 46, 16)
                                .Provincia_Estera = getStringParameter(theSqlDataReader, 47)
                                .IndirizzoSenzaNumero = getStringParameter(theSqlDataReader, 48)
                                .NumeroCivico = getStringParameter(theSqlDataReader, 49)
                                .Data_Nascita = getDataTimeParameter(theSqlDataReader, 50)
                                .StRit_Acconto = getIntegerParameter(theSqlDataReader, 51, 16)
                                .CodTributo = getIntegerParameter(theSqlDataReader, 52, 32)
                                .Codice_SEDE = getStringParameter(theSqlDataReader, 53)
                                .Codice_Costo = getStringParameter(theSqlDataReader, 54)
                                .NoFatt = getIntegerParameter(theSqlDataReader, 55, 16)
                                .CIN = getStringParameter(theSqlDataReader, 56)
                                .NazIBAN = getStringParameter(theSqlDataReader, 57)
                                .CINEUIBAN = getStringParameter(theSqlDataReader, 58)
                                .SWIFT = getStringParameter(theSqlDataReader, 59)
                                .Listino = getIntegerParameter(theSqlDataReader, 60, 32)
                                .CSAggrAllIVA = getBooleanParameter(theSqlDataReader, 61)
                                .IBAN_Ditta = getStringParameter(theSqlDataReader, 62) 'Pier220612
                                .PECEMail = getStringParameter(theSqlDataReader, 63)
                            End With

                            myArray.Add(myFornitore)
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
        'giu121211
        Public Function CIFornitoreByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements FornitoreDAO.CIFornitoreByCodice
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
                theSqlCommand.CommandText = "CI_FornitoreByCodice"
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
        Public Function CIFornitoreByCodiceAZI(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements FornitoreDAO.CIFornitoreByCodiceAZI
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
                theSqlCommand.CommandText = "CI_FornitoreByCodiceAZI"
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
        Public Function CIFornitoreByCodiceSCAD(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements FornitoreDAO.CIFornitoreByCodiceSCAD
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
                theSqlCommand.CommandText = "CI_FofnitoreByCodiceSCAD"
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
        Public Function delFornitoriByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements FornitoreDAO.delFornitoriByCodice
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
                theSqlCommand.CommandText = "del_FornitoreByCodice"
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
        Public Function InsertUpdateFornitore(ByVal idTransazione As Integer, ByVal paramFornitori As Object) As Boolean Implements FornitoreDAO.InsertUpdateFornitore
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim theError As Integer
            Dim bReturnOk As Boolean
            Dim myFornitoriEntity As FornitoreEntity = CType(paramFornitori, FornitoreEntity)

            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "InsertUpdate_Fornitore"
                param = setParameter(param, theSqlCommand, "@Codice_CoGe", SqlDbType.NVarChar, myFornitoriEntity.Codice_CoGe, 16)
                param = setParameter(param, theSqlCommand, "@Rag_Soc", SqlDbType.NVarChar, myFornitoriEntity.Rag_Soc, 50)
                param = setParameter(param, theSqlCommand, "@Indirizzo", SqlDbType.NVarChar, myFornitoriEntity.Indirizzo, 50)
                param = setParameter(param, theSqlCommand, "@Localita", SqlDbType.NVarChar, myFornitoriEntity.Localita, 50)
                param = setParameter(param, theSqlCommand, "@CAP", SqlDbType.NVarChar, myFornitoriEntity.CAP, 5)
                param = setParameter(param, theSqlCommand, "@Provincia", SqlDbType.NVarChar, myFornitoriEntity.Provincia, 2)
                param = setParameter(param, theSqlCommand, "@Nazione", SqlDbType.NVarChar, myFornitoriEntity.Nazione, 3)
                param = setParameter(param, theSqlCommand, "@Telefono1", SqlDbType.NVarChar, myFornitoriEntity.Telefono1, 30)
                param = setParameter(param, theSqlCommand, "@Telefono2", SqlDbType.NVarChar, myFornitoriEntity.Telefono2, 30)
                param = setParameter(param, theSqlCommand, "@Fax", SqlDbType.NVarChar, myFornitoriEntity.Fax, 30)
                param = setParameter(param, theSqlCommand, "@Societa", SqlDbType.Int, myFornitoriEntity.Societa)
                param = setParameter(param, theSqlCommand, "@Codice_Fiscale", SqlDbType.NVarChar, myFornitoriEntity.Codice_Fiscale, 16)
                param = setParameter(param, theSqlCommand, "@Partita_IVA", SqlDbType.NVarChar, myFornitoriEntity.Partita_IVA, 20) 'giu160320
                param = setParameter(param, theSqlCommand, "@Zona", SqlDbType.Int, myFornitoriEntity.Zona)
                param = setParameter(param, theSqlCommand, "@Categoria", SqlDbType.Int, myFornitoriEntity.Categoria)
                param = setParameter(param, theSqlCommand, "@ABI_N", SqlDbType.NVarChar, myFornitoriEntity.ABI_N, 5)
                param = setParameter(param, theSqlCommand, "@CAB_N", SqlDbType.NVarChar, myFornitoriEntity.CAB_N, 5)
                param = setParameter(param, theSqlCommand, "@Regime_IVA", SqlDbType.Int, myFornitoriEntity.Regime_IVA)
                'param = setParameter(param, theSqlCommand, "@Agente_N", SqlDbType.Int, myFornitoriEntity.Agente_N)
                'param = setParameter(param, theSqlCommand, "@Vettore_N", SqlDbType.Int, myFornitoriEntity.Vettore_N)
                param = setParameter(param, theSqlCommand, "@Pagamento_N", SqlDbType.Int, myFornitoriEntity.Pagamento_N)
                param = setParameter(param, theSqlCommand, "@Ragg_P", SqlDbType.Int, myFornitoriEntity.Ragg_P)
                param = setParameter(param, theSqlCommand, "@Bilancio_SN", SqlDbType.NVarChar, myFornitoriEntity.Bilancio_SN, 1)
                param = setParameter(param, theSqlCommand, "@Allegato_IVA", SqlDbType.Int, myFornitoriEntity.Allegato_IVA)
                param = setParameter(param, theSqlCommand, "@Apertura", SqlDbType.Decimal, myFornitoriEntity.Apertura, 10)
                param = setParameter(param, theSqlCommand, "@DA_Apertura", SqlDbType.NVarChar, myFornitoriEntity.DA_Apertura, 1)
                param = setParameter(param, theSqlCommand, "@Saldo_Dare", SqlDbType.Decimal, myFornitoriEntity.Saldo_Dare, 10)
                param = setParameter(param, theSqlCommand, "@Saldo_Avere", SqlDbType.Decimal, myFornitoriEntity.Saldo_Avere, 10)
                param = setParameter(param, theSqlCommand, "@Data_Agg_Saldi", SqlDbType.DateTime, myFornitoriEntity.Data_Agg_Saldi, 10)
                param = setParameter(param, theSqlCommand, "@Saldo_Prec", SqlDbType.Decimal, myFornitoriEntity.Saldo_Prec, 10)
                param = setParameter(param, theSqlCommand, "@DA_Saldo_Prec", SqlDbType.NVarChar, myFornitoriEntity.DA_Saldo_Prec, 1)
                param = setParameter(param, theSqlCommand, "@Dare_Chiusura", SqlDbType.Decimal, myFornitoriEntity.Dare_Chiusura, 10)
                param = setParameter(param, theSqlCommand, "@Avere_Chiusura", SqlDbType.Decimal, myFornitoriEntity.Avere_Chiusura, 10)
                param = setParameter(param, theSqlCommand, "@Riferimento", SqlDbType.NVarChar, myFornitoriEntity.Riferimento, 50)
                param = setParameter(param, theSqlCommand, "@Denominazione", SqlDbType.NVarChar, myFornitoriEntity.Denominazione, 50)
                param = setParameter(param, theSqlCommand, "@Titolare", SqlDbType.NVarChar, myFornitoriEntity.Titolare, 50)
                param = setParameter(param, theSqlCommand, "@Email", SqlDbType.NVarChar, myFornitoriEntity.Email, 100)
                param = setParameter(param, theSqlCommand, "@Saldo_Dare_2", SqlDbType.Decimal, myFornitoriEntity.Saldo_Dare_2, 10)
                param = setParameter(param, theSqlCommand, "@Saldo_Avere_2", SqlDbType.Decimal, myFornitoriEntity.Saldo_Avere_2, 10)
                param = setParameter(param, theSqlCommand, "@Dare_Chiusura_2", SqlDbType.Decimal, myFornitoriEntity.Dare_Chiusura_2, 10)
                param = setParameter(param, theSqlCommand, "@Avere_Chiusura_2", SqlDbType.Decimal, myFornitoriEntity.Avere_Chiusura_2, 10)
                param = setParameter(param, theSqlCommand, "@Apertura_2", SqlDbType.Decimal, myFornitoriEntity.Apertura_2, 10)
                param = setParameter(param, theSqlCommand, "@DA_Apertura_2", SqlDbType.NVarChar, myFornitoriEntity.DA_Apertura_2, 1)
                'param = setParameter(param, theSqlCommand, "@DEM", SqlDbType.Int, myFornitoriEntity.DEM, 4)
                'param = setParameter(param, theSqlCommand, "@Contenitori", SqlDbType.Int, myFornitoriEntity.Contenitori, 4)
                'param = setParameter(param, theSqlCommand, "@Inviato", SqlDbType.Int, myFornitoriEntity.Inviato, 4)
                param = setParameter(param, theSqlCommand, "@Listino", SqlDbType.Int, myFornitoriEntity.Listino, 4)
                param = setParameter(param, theSqlCommand, "@Conto_Corrente", SqlDbType.NVarChar, myFornitoriEntity.Conto_Corrente, 15)
                'param = setParameter(param, theSqlCommand, "@Credito_1", SqlDbType.Decimal, myFornitoriEntity.Credito_1, 10)
                'param = setParameter(param, theSqlCommand, "@Credito_2", SqlDbType.Decimal, myFornitoriEntity.Credito_2, 10)
                param = setParameter(param, theSqlCommand, "@Provincia_Estera", SqlDbType.NVarChar, myFornitoriEntity.Provincia_Estera, 15)
                param = setParameter(param, theSqlCommand, "@IndirizzoSenzaNumero", SqlDbType.NVarChar, myFornitoriEntity.IndirizzoSenzaNumero, 50)
                param = setParameter(param, theSqlCommand, "@NumeroCivico", SqlDbType.NVarChar, myFornitoriEntity.NumeroCivico, 10)
                'param = setParameter(param, theSqlCommand, "@GiornoChiusura_1", SqlDbType.Int, myFornitoriEntity.GiornoChiusura_1, 4)
                'param = setParameter(param, theSqlCommand, "@GiornoChiusura_2", SqlDbType.Int, myFornitoriEntity.GiornoChiusura_2, 4)
                'param = setParameter(param, theSqlCommand, "@ChiusuraMattino_1", SqlDbType.Int, myFornitoriEntity.ChiusuraMattino_1, 4)
                'param = setParameter(param, theSqlCommand, "@ChiusuraPomeriggio_1", SqlDbType.Int, myFornitoriEntity.ChiusuraPomeriggio_1, 4)
                'param = setParameter(param, theSqlCommand, "@ChiusuraMattino_2", SqlDbType.Int, myFornitoriEntity.ChiusuraMattino_2, 4)
                'param = setParameter(param, theSqlCommand, "@ChiusuraPomeriggio_2", SqlDbType.Int, myFornitoriEntity.ChiusuraPomeriggio_2, 4)
                'param = setParameter(param, theSqlCommand, "@Note", SqlDbType.NText, myFornitoriEntity.Note)
                'param = setParameter(param, theSqlCommand, "@CodProvinciaAgente", SqlDbType.NVarChar, myFornitoriEntity.CodProvinciaAgente, 5)
                'param = setParameter(param, theSqlCommand, "@Agente_N_Prec", SqlDbType.Int, myFornitoriEntity.Agente_N_Prec, 4)
                param = setParameter(param, theSqlCommand, "@Codice_SEDE", SqlDbType.NVarChar, myFornitoriEntity.Codice_SEDE, 16)
                param = setParameter(param, theSqlCommand, "@Codice_Costo", SqlDbType.NVarChar, myFornitoriEntity.Codice_Costo, 16)
                'param = setParameter(param, theSqlCommand, "@IVASosp", SqlDbType.Int, myFornitoriEntity.IVASosp, 4)
                param = setParameter(param, theSqlCommand, "@NoFatt", SqlDbType.Int, myFornitoriEntity.NoFatt, 4)
                param = setParameter(param, theSqlCommand, "@CIN", SqlDbType.NVarChar, myFornitoriEntity.CIN, 1)
                param = setParameter(param, theSqlCommand, "@Data_Nascita", SqlDbType.DateTime, myFornitoriEntity.Data_Nascita, 10)
                param = setParameter(param, theSqlCommand, "@NazIBAN", SqlDbType.NVarChar, myFornitoriEntity.NazIBAN, 3)
                param = setParameter(param, theSqlCommand, "@CINEUIBAN", SqlDbType.NVarChar, myFornitoriEntity.CINEUIBAN, 2)
                param = setParameter(param, theSqlCommand, "@SWIFT", SqlDbType.NVarChar, myFornitoriEntity.SWIFT, 15)
                param = setParameter(param, theSqlCommand, "@CSAggrAllIVA", SqlDbType.Bit, myFornitoriEntity.CSAggrAllIVA, 1)
                param = setParameter(param, theSqlCommand, "@InseritoDa", SqlDbType.NVarChar, myFornitoriEntity.InseritoDa, 50)
                param = setParameter(param, theSqlCommand, "@ModificatoDa", SqlDbType.NVarChar, myFornitoriEntity.ModificatoDa, 50)
                param = setParameter(param, theSqlCommand, "@IBAN_Ditta", SqlDbType.NVarChar, myFornitoriEntity.IBAN_Ditta, 27) 'Pier220612
                param = setParameter(param, theSqlCommand, "@PECEMail", SqlDbType.NVarChar, myFornitoriEntity.PECEMail, 310)
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
    End Class
End Namespace
