Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.SystemFramework
Imports System.Text

Namespace It.SoftAzi.Model.Roles
    Public Class OperatoriRole
        Public Function getOperatoriByName(ByVal nome As String) As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myOperatori As OperatoriDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myOperatori = myFactory.getOperatoriByName
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbInstall)
                ptrns = myOperatori.getOperatoriByName(myIdTransazione, nome)
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

        Public Function UpdOperatoriDataOraUltAccesso(ByVal Nome As String) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myOperatori As OperatoriDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False
            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myOperatori = myFactory.getOperatoriByName
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbInstall)
                bInsertOk = myOperatori.UpdOperatoriDataOraUltAccesso(myIdTransazione, Nome)
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

        Public Function OperatoreConnesso(ByVal Nome As String, ByVal CodiceDitta As String, ByVal Postazione As String, ByVal Modulo As String, ByVal SessionID As String, ByVal ID As Integer, ByVal Codice As String, ByVal Azienda As String, ByVal Tipo As String, ByVal _Esercizio As String) As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myOperatori As OperatoriDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList
            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myOperatori = myFactory.getOperatoriByName
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbInstall)
                ptrns = myOperatori.OperatoreConnesso(myIdTransazione, Nome, CodiceDitta, Postazione, Modulo, SessionID, ID, Codice, Azienda, Tipo, _Esercizio)
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
        Public Function DelOperatoreConnesso(ByVal Nome As String, ByVal CodiceDitta As String, ByVal Modulo As String) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myOperatori As OperatoriDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False
            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myOperatori = myFactory.getOperatoriByName
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbInstall)
                bInsertOk = myOperatori.DelOperatoreConnesso(myIdTransazione, Nome, CodiceDitta, Modulo)
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

        Public Function UpdOperatoriDataOraUltAzione(ByVal Nome As String) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myOperatori As OperatoriDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False
            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myOperatori = myFactory.getOperatoriByName
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbInstall)
                bInsertOk = myOperatori.UpdOperatoriDataOraUltAzione(myIdTransazione, Nome)
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