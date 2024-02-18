Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.SystemFramework
Imports System.Text

Namespace It.SoftAzi.Model.Roles
    Public Class AnaMagRole
        Public Function getAnaMag() As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myAnaMag As AnaMagDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myAnaMag = myFactory.getAnaMagByCodice
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                ptrns = myAnaMag.getAnaMag(myIdTransazione)
            Catch ex As Exception
                bCommit = False
                Throw ex
                'If log.IsErrorEnabled Then
                '    log.Error("Eccezione: " & ex.Message)
                'End If
            Finally
                If (myIdTransazione <> -1) Then
                    myDataSource.endTransaction(myIdTransazione, bCommit)
                End If
            End Try
            Return ptrns
        End Function
        Public Function getAnaMagByCodice(ByVal Codice As String) As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myAnaMag As AnaMagDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myAnaMag = myFactory.getAnaMagByCodice
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                ptrns = myAnaMag.getAnaMagByCodice(myIdTransazione, Codice)
            Catch ex As Exception
                bCommit = False
                Throw ex
                'If log.IsErrorEnabled Then
                '    log.Error("Eccezione: " & ex.Message)
                'End If
            Finally
                If (myIdTransazione <> -1) Then
                    myDataSource.endTransaction(myIdTransazione, bCommit)
                End If
            End Try
            Return ptrns
        End Function
        Public Function delAnaMagByCodice(ByVal Codice As String) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myAnaMag As AnaMagDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myAnaMag = myFactory.getAnaMagByCodice
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                bInsertOk = myAnaMag.delAnaMagByCodice(myIdTransazione, Codice)
            Catch ex As Exception
                bCommit = False
                Throw ex
                'If log.IsErrorEnabled Then
                '    log.Error("Eccezione: " & ex.Message)
                'End If
            Finally
                If (myIdTransazione <> -1) Then
                    myDataSource.endTransaction(myIdTransazione, bCommit)
                End If
            End Try
            Return bInsertOk
        End Function
        Public Function InsertUpdateAnaMag(ByVal myAnaMagEntity As Object) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myAnaMag As AnaMagDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False

            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myAnaMag = myFactory.getAnaMagByCodice
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                bInsertOk = myAnaMag.InsertUpdateAnaMag(myIdTransazione, myAnaMagEntity)
            Catch ex As Exception
                bCommit = False
                Throw ex
                'If log.IsErrorEnabled Then
                '    log.Error("Eccezione: " & ex.Message)
                'End If
            Finally
                If (myIdTransazione <> -1) Then
                    myDataSource.endTransaction(myIdTransazione, bCommit)
                End If
            End Try
            Return bInsertOk
        End Function
        Public Function CIAnaMagByCodice(ByVal Codice As String) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myAnaMag As AnaMagDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myAnaMag = myFactory.getAnaMagByCodice
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                bInsertOk = myAnaMag.CIAnaMagByCodice(myIdTransazione, Codice)
            Catch ex As Exception
                bCommit = False
                Throw ex
                'If log.IsErrorEnabled Then
                '    log.Error("Eccezione: " & ex.Message)
                'End If
            Finally
                If (myIdTransazione <> -1) Then
                    myDataSource.endTransaction(myIdTransazione, bCommit)
                End If
            End Try
            Return bInsertOk
        End Function
    End Class
End Namespace

