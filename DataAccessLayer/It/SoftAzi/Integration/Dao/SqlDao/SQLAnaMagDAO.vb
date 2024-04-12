Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Collections
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports It.SoftAzi.SystemFramework.FileSystemUtil
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLAnaMagDAO
        Inherits SQLBaseDAO
        Implements It.SoftAzi.Integration.Dao.AnaMagDAO
        Public Function getAnaMag(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements AnaMagDAO.getAnaMag
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myAnaMag As AnaMagEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_AnaMag"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myAnaMag = New AnaMagEntity
                            With myAnaMag
                                .CodArticolo = getStringParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .Altezza = getDecimalParameter(theSqlDataReader, 2)
                                .ArticoloDiVendita = getBooleanParameter(theSqlDataReader, 3)
                                .Autore = getStringParameter(theSqlDataReader, 4)
                                .AvvisaSottoscorta = getBooleanParameter(theSqlDataReader, 5)
                                .Categoria = getStringParameter(theSqlDataReader, 6)
                                .TipoCodice = getStringParameter(theSqlDataReader, 7)
                                .CodArticoloEAN = getStringParameter(theSqlDataReader, 8)
                                .CodAziendaEAN = getStringParameter(theSqlDataReader, 9)
                                .CodBarre = getStringParameter(theSqlDataReader, 10)
                                .CodControlloEAN = getStringParameter(theSqlDataReader, 11)
                                .CodiceDelFornitore = getStringParameter(theSqlDataReader, 12)
                                .CodiceFornitore = getStringParameter(theSqlDataReader, 13)
                                .CodiceIntra = getStringParameter(theSqlDataReader, 14)
                                .CodImba1 = getStringParameter(theSqlDataReader, 15)
                                .CodImba2 = getStringParameter(theSqlDataReader, 16)
                                .CodIva = getIntegerParameter(theSqlDataReader, 17)
                                .CodPagamento = getIntegerParameter(theSqlDataReader, 18)
                                .CodPeso = getIntegerParameter(theSqlDataReader, 19)
                                .Confezionato = getDecimalParameter(theSqlDataReader, 20)
                                .Confezione = getIntegerParameter(theSqlDataReader, 21)
                                .DataFineProd = getDataTimeParameter(theSqlDataReader, 22)
                                .DataInizioProd = getDataTimeParameter(theSqlDataReader, 23)
                                .TipoArticolo = getIntegerParameter(theSqlDataReader, 24)
                                .GestLotti = getBooleanParameter(theSqlDataReader, 25)
                                .Giacenza = getDecimalParameter(theSqlDataReader, 26)
                                .GiorniConsegna = getIntegerParameter(theSqlDataReader, 27)
                                .TipoCodice = getStringParameter(theSqlDataReader, 28)
                                .Um = getStringParameter(theSqlDataReader, 29)
                                .OrdClienti = getDecimalParameter(theSqlDataReader, 30)
                                .OrdFornit = getDecimalParameter(theSqlDataReader, 31)
                                .PesoUnitario = getDecimalParameter(theSqlDataReader, 32)
                                .Linea = getStringParameter(theSqlDataReader, 33)
                                .Reparto = getIntegerParameter(theSqlDataReader, 34)
                                .Scaffale = getIntegerParameter(theSqlDataReader, 35)
                                .Piano = getIntegerParameter(theSqlDataReader, 36)
                                .Lunghezza = getDecimalParameter(theSqlDataReader, 37)
                                .Larghezza = getDecimalParameter(theSqlDataReader, 38)
                                .Sottoscorta = getDecimalParameter(theSqlDataReader, 39)
                                .NConfTipo1 = getIntegerParameter(theSqlDataReader, 40)
                                .NConfTipo2 = getIntegerParameter(theSqlDataReader, 41)
                                .Prodotto = getDecimalParameter(theSqlDataReader, 42)
                                .Ordinato = getDecimalParameter(theSqlDataReader, 43)
                                .Venduto = getDecimalParameter(theSqlDataReader, 44)
                                .Ricarico = getDecimalParameter(theSqlDataReader, 45)
                                .QtaOrdine = getDecimalParameter(theSqlDataReader, 46)
                                .PrezzoAcquisto = getDecimalParameter(theSqlDataReader, 47)
                                .LBase = getIntegerParameter(theSqlDataReader, 48)
                                .LOpz = getIntegerParameter(theSqlDataReader, 49)
                                .NAnniGaranzia = getIntegerParameter(theSqlDataReader, 50)
                                .NAnniScadElettrodi = getIntegerParameter(theSqlDataReader, 51)
                                .NAnniScadBatterie = getIntegerParameter(theSqlDataReader, 52)
                                .ScFornitore = getDecimalParameter(theSqlDataReader, 53)
                                .IDModulo1 = getIntegerParameter(theSqlDataReader, 54)
                                .IDModulo2 = getIntegerParameter(theSqlDataReader, 55)
                                .IDModulo3 = getIntegerParameter(theSqlDataReader, 56)
                                .IDModulo4 = getIntegerParameter(theSqlDataReader, 57)
                            End With

                            myArray.Add(myAnaMag)
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
        Public Function getAnaMagByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As System.Collections.ArrayList Implements AnaMagDAO.getAnaMagByCodice
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myAnaMag As AnaMagEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_AnaMagByCodice"
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
                            myAnaMag = New AnaMagEntity
                            With myAnaMag
                                .CodArticolo = getStringParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                                .Altezza = getDecimalParameter(theSqlDataReader, 2)
                                .ArticoloDiVendita = getBooleanParameter(theSqlDataReader, 3)
                                .Autore = getStringParameter(theSqlDataReader, 4)
                                .AvvisaSottoscorta = getBooleanParameter(theSqlDataReader, 5)
                                .Categoria = getStringParameter(theSqlDataReader, 6)
                                .TipoCodice = getStringParameter(theSqlDataReader, 7)
                                .CodArticoloEAN = getStringParameter(theSqlDataReader, 8)
                                .CodAziendaEAN = getStringParameter(theSqlDataReader, 9)
                                .CodBarre = getStringParameter(theSqlDataReader, 10)
                                .CodControlloEAN = getStringParameter(theSqlDataReader, 11)
                                .CodiceDelFornitore = getStringParameter(theSqlDataReader, 12)
                                .CodiceFornitore = getStringParameter(theSqlDataReader, 13)
                                .CodiceIntra = getStringParameter(theSqlDataReader, 14)
                                .CodImba1 = getStringParameter(theSqlDataReader, 15)
                                .CodImba2 = getStringParameter(theSqlDataReader, 16)
                                .CodIva = getIntegerParameter(theSqlDataReader, 17)
                                .CodPagamento = getIntegerParameter(theSqlDataReader, 18)
                                .CodPeso = getIntegerParameter(theSqlDataReader, 19)
                                .Confezionato = getDecimalParameter(theSqlDataReader, 20)
                                .Confezione = getIntegerParameter(theSqlDataReader, 21)
                                .DataFineProd = getDataTimeParameter(theSqlDataReader, 22)
                                .DataInizioProd = getDataTimeParameter(theSqlDataReader, 23)
                                .TipoArticolo = getIntegerParameter(theSqlDataReader, 24)
                                .GestLotti = getBooleanParameter(theSqlDataReader, 25)
                                .Giacenza = getDecimalParameter(theSqlDataReader, 26)
                                .GiorniConsegna = getIntegerParameter(theSqlDataReader, 27)
                                .TipoCodice = getStringParameter(theSqlDataReader, 28)
                                .Um = getStringParameter(theSqlDataReader, 29)
                                .OrdClienti = getDecimalParameter(theSqlDataReader, 30)
                                .OrdFornit = getDecimalParameter(theSqlDataReader, 31)
                                .PesoUnitario = getDecimalParameter(theSqlDataReader, 32)
                                .Linea = getStringParameter(theSqlDataReader, 33)
                                .Reparto = getIntegerParameter(theSqlDataReader, 34)
                                .Scaffale = getIntegerParameter(theSqlDataReader, 35)
                                .Piano = getIntegerParameter(theSqlDataReader, 36)
                                .Lunghezza = getDecimalParameter(theSqlDataReader, 37)
                                .Larghezza = getDecimalParameter(theSqlDataReader, 38)
                                .Sottoscorta = getDecimalParameter(theSqlDataReader, 39)
                                .NConfTipo1 = getIntegerParameter(theSqlDataReader, 40)
                                .NConfTipo2 = getIntegerParameter(theSqlDataReader, 41)
                                .Prodotto = getDecimalParameter(theSqlDataReader, 42)
                                .Ordinato = getDecimalParameter(theSqlDataReader, 43)
                                .Venduto = getDecimalParameter(theSqlDataReader, 44)
                                .Ricarico = getDecimalParameter(theSqlDataReader, 45)
                                .QtaOrdine = getDecimalParameter(theSqlDataReader, 46)
                                .PrezzoAcquisto = getDecimalParameter(theSqlDataReader, 47)
                                .LBase = getIntegerParameter(theSqlDataReader, 48)
                                .LOpz = getIntegerParameter(theSqlDataReader, 49)
                                .NAnniGaranzia = getIntegerParameter(theSqlDataReader, 50)
                                .NAnniScadElettrodi = getIntegerParameter(theSqlDataReader, 51)
                                .NAnniScadBatterie = getIntegerParameter(theSqlDataReader, 52)
                                .ScFornitore = getDecimalParameter(theSqlDataReader, 53)
                                .IDModulo1 = getIntegerParameter(theSqlDataReader, 54)
                                .IDModulo2 = getIntegerParameter(theSqlDataReader, 55)
                                .IDModulo3 = getIntegerParameter(theSqlDataReader, 56)
                                .IDModulo4 = getIntegerParameter(theSqlDataReader, 57)
                            End With

                            myArray.Add(myAnaMag)
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
        Public Function InsertUpdateAnaMag(ByVal idTransazione As Integer, ByVal myAnaMag As Object) As Boolean Implements AnaMagDAO.InsertUpdateAnaMag
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim theError As Integer
            Dim bReturnOk As Boolean
            Dim myAnaMagEntity As AnaMagEntity = CType(myAnaMag, AnaMagEntity)

            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "InsertUpdate_AnaMag"
                param = setParameter(param, theSqlCommand, "@Cod_Articolo", SqlDbType.NVarChar, myAnaMagEntity.CodArticolo, 20)
                param = setParameter(param, theSqlCommand, "@Descrizione", SqlDbType.NVarChar, myAnaMagEntity.Descrizione, 150)
                param = setParameter(param, theSqlCommand, "@Altezza", SqlDbType.Decimal, myAnaMagEntity.Altezza, 9)
                param = setParameter(param, theSqlCommand, "@Articolo_di_vendita", SqlDbType.Bit, myAnaMagEntity.ArticoloDiVendita, 1)
                param = setParameter(param, theSqlCommand, "@Autore", SqlDbType.NVarChar, myAnaMagEntity.Autore, 50)
                param = setParameter(param, theSqlCommand, "@Avvisa_Sottoscorta", SqlDbType.Bit, myAnaMagEntity.AvvisaSottoscorta, 1)
                param = setParameter(param, theSqlCommand, "@Categoria", SqlDbType.NVarChar, myAnaMagEntity.Categoria, 3)
                param = setParameter(param, theSqlCommand, "@Tipo_Codice", SqlDbType.NVarChar, myAnaMagEntity.TipoCodice, 10)
                param = setParameter(param, theSqlCommand, "@Cod_Barre", SqlDbType.NVarChar, myAnaMagEntity.CodBarre, 20)
                param = setParameter(param, theSqlCommand, "@CodiceDelFornitore", SqlDbType.NVarChar, myAnaMagEntity.CodiceDelFornitore, 20)
                param = setParameter(param, theSqlCommand, "@CodiceFornitore", SqlDbType.NVarChar, myAnaMagEntity.CodiceFornitore, 16)
                param = setParameter(param, theSqlCommand, "@Cod_Iva", SqlDbType.Int, myAnaMagEntity.CodIva)
                param = setParameter(param, theSqlCommand, "@Cod_Pagamento", SqlDbType.Int, myAnaMagEntity.CodPagamento)
                param = setParameter(param, theSqlCommand, "@Confezione", SqlDbType.Int, myAnaMagEntity.Confezione)
                param = setParameter(param, theSqlCommand, "@TipoArticolo", SqlDbType.Int, myAnaMagEntity.TipoArticolo)
                param = setParameter(param, theSqlCommand, "@Gest_Lotti", SqlDbType.Bit, myAnaMagEntity.GestLotti, 1)
                param = setParameter(param, theSqlCommand, "@Giorni_Consegna", SqlDbType.Int, myAnaMagEntity.GiorniConsegna)
                param = setParameter(param, theSqlCommand, "@Um", SqlDbType.NVarChar, myAnaMagEntity.Um, 2)
                param = setParameter(param, theSqlCommand, "@Peso_Unitario", SqlDbType.Decimal, myAnaMagEntity.PesoUnitario, 9)
                param = setParameter(param, theSqlCommand, "@Linea", SqlDbType.NVarChar, myAnaMagEntity.Linea, 3)
                param = setParameter(param, theSqlCommand, "@Lunghezza", SqlDbType.Decimal, myAnaMagEntity.Lunghezza, 9)
                param = setParameter(param, theSqlCommand, "@Larghezza", SqlDbType.Decimal, myAnaMagEntity.Larghezza, 9)
                param = setParameter(param, theSqlCommand, "@Sottoscorta", SqlDbType.Decimal, myAnaMagEntity.Sottoscorta, 9)
                param = setParameter(param, theSqlCommand, "@Ricarico", SqlDbType.Decimal, myAnaMagEntity.Ricarico, 9)
                param = setParameter(param, theSqlCommand, "@Qta_Ordine", SqlDbType.Decimal, myAnaMagEntity.QtaOrdine, 9)
                param = setParameter(param, theSqlCommand, "@PrezzoAcquisto", SqlDbType.Decimal, myAnaMagEntity.PrezzoAcquisto) 'giu110512 , 9)
                param = setParameter(param, theSqlCommand, "@LBase", SqlDbType.Int, myAnaMagEntity.LBase)
                param = setParameter(param, theSqlCommand, "@LOpz", SqlDbType.Int, myAnaMagEntity.LOpz)
                param = setParameter(param, theSqlCommand, "@NAnniGaranzia", SqlDbType.Int, myAnaMagEntity.NAnniGaranzia)
                param = setParameter(param, theSqlCommand, "@NAnniScadElettrodi", SqlDbType.Int, myAnaMagEntity.NAnniScadElettrodi)
                param = setParameter(param, theSqlCommand, "@NAnniScadBatterie", SqlDbType.Int, myAnaMagEntity.NAnniScadBatterie)
                param = setParameter(param, theSqlCommand, "@ScFornitore", SqlDbType.Decimal, myAnaMagEntity.ScFornitore)
                param = setParameter(param, theSqlCommand, "@IDModulo1", SqlDbType.Int, myAnaMagEntity.IDModulo1)
                param = setParameter(param, theSqlCommand, "@IDModulo2", SqlDbType.Int, myAnaMagEntity.IDModulo2)
                param = setParameter(param, theSqlCommand, "@IDModulo3", SqlDbType.Int, myAnaMagEntity.IDModulo3)
                param = setParameter(param, theSqlCommand, "@IDModulo4", SqlDbType.Int, myAnaMagEntity.IDModulo4)
                param = theSqlCommand.Parameters.Add("@RetVal", SqlDbType.Int)
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
        Public Function delAnaMagByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements AnaMagDAO.delAnaMagByCodice
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
                theSqlCommand.CommandText = "del_AnaMagByCodice"
                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.NVarChar, 20)
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
        Public Function CIAnaMagByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean Implements AnaMagDAO.CIAnaMagByCodice
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
                theSqlCommand.CommandText = "CI_AnaMagByCodice"
                param = theSqlCommand.Parameters.Add("@Codice", SqlDbType.NVarChar, 20)
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
        'giu110424 per velocizzare i caricamenti iniziali FARLO ANCHE PER GLI ARTICOLI
        Public Function getAnaMagCodDes(ByVal idTransazione As Integer) As System.Collections.ArrayList Implements AnaMagDAO.getAnaMagCodDes
            Dim theSqlCommand As SqlCommand
            Dim param As SqlParameter = Nothing
            Dim theSqlDataReader As SqlDataReader = Nothing
            Dim myAnaMag As AnaMagEntity
            Dim myArray As ArrayList
            Dim theError As Integer
            Try
                Try
                    theSqlCommand = MyBase.getCommnand(idTransazione)
                Catch sex As SqlException
                    Throw sex
                End Try
                theSqlCommand.CommandType = CommandType.StoredProcedure
                theSqlCommand.CommandText = "get_AnaMagCodDes"
                param = theSqlCommand.Parameters.Add("RETURN_VALUE", SqlDbType.Int)
                param.Direction = ParameterDirection.ReturnValue
                Try
                    theSqlDataReader = theSqlCommand.ExecuteReader()
                    theError = CInt(theSqlCommand.Parameters("RETURN_VALUE").Value())
                    If theError = 0 Then
                        myArray = New ArrayList
                        While (theSqlDataReader.Read())
                            myAnaMag = New AnaMagEntity
                            With myAnaMag
                                .CodArticolo = getStringParameter(theSqlDataReader, 0)
                                .Descrizione = getStringParameter(theSqlDataReader, 1)
                            End With

                            myArray.Add(myAnaMag)
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
