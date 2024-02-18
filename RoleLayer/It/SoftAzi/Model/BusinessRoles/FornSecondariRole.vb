Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.SystemFramework
Imports System.Text

Namespace It.SoftAzi.Model.Roles
    Public Class FornSecondariRole
        Public Function getFornSecondariByCodiceArticolo(ByVal Codice As String) As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myFornSecondari As FornSecondariDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList

            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myFornSecondari = myFactory.getFornSecondariByCodiceArticolo
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                ptrns = myFornSecondari.getFornSecondariByCodiceArticolo(myIdTransazione, Codice)
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
        Public Function delFornSecByCodiceArticolo(ByVal Codice As String) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myFornSec As FornSecondariDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myFornSec = myFactory.getFornSecondariByCodiceArticolo
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                bInsertOk = myFornSec.delFornSecByCodiceArticolo(myIdTransazione, Codice)
            Catch ex As Exception
                bCommit = False
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
        Public Function InsertFornitoriSec(ByVal myFornSecEntity As Object) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myFornSec As FornSecondariDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False

            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myFornSec = myFactory.getFornSecondariByCodiceArticolo
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                bInsertOk = myFornSec.InsertFornitoriSec(myIdTransazione, myFornSecEntity)
            Catch ex As Exception
                bCommit = False
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

