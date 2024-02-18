Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.SystemFramework
Imports System.Text

Namespace It.SoftAzi.Model.Roles
    Public Class AliquoteIvaRole
        Public Function getAliquoteIva() As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myAliquoteIva As AliquoteIvaDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList

            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myAliquoteIva = myFactory.getAliquoteIva
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbSoftCoge)
                ptrns = myAliquoteIva.getAliquoteIva(myIdTransazione)
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

