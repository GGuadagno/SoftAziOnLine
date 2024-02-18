Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.SystemFramework
Imports System.Text

Namespace It.SoftAzi.Model.Roles
    Public Class ListVenDRole
        Public Function getListVenDByCodLisCodArt(ByVal CodLis As Integer, ByVal CodArt As String) As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myListVenD As ListVenDDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myListVenD = myFactory.getListVenDByCodLisCodArt
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                ptrns = myListVenD.getListVenDByCodLisCodArt(myIdTransazione, CodLis, CodArt)
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
        Public Function InsertUpdateListVenD(ByVal myListVenDEntity As Object) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myListVenD As ListVenDDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False

            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myListVenD = myFactory.getListVenDByCodLisCodArt
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                bInsertOk = myListVenD.InsertUpdateListVenD(myIdTransazione, myListVenDEntity)
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
        Public Function InsertiArticoliOu(ByVal CodLis As Integer) As Boolean
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myListVenD As ListVenDDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim bInsertOk As Boolean = False

            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myListVenD = myFactory.getListVenDByCodLisCodArt
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftAzi)
                bInsertOk = myListVenD.InsertiArticoliOu(myIdTransazione, CodLis)
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
