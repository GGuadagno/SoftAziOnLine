Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.SystemFramework
Imports System.Text

Namespace It.SoftAzi.Model.Roles
    Public Class AgentiRole
        Public Function getAgenti() As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myAgenti As AgentiDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList

            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myAgenti = myFactory.getAgenti
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftCoge)
                ptrns = myAgenti.getAgenti(myIdTransazione)
            Catch ex As Exception
                bCommit = False
                Throw ex
            Finally
                If (myIdTransazione <> -1) Then
                    myDataSource.endTransaction(myIdTransazione, bCommit)
                End If
            End Try
            Return ptrns
        End Function
    End Class
End Namespace

