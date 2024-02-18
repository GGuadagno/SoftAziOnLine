﻿Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.SystemFramework
Imports System.Text

Namespace It.SoftAzi.Model.Roles
    Public Class DitteRole
        Public Function getDitte() As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myDitte As DitteDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myDitte = myFactory.getDitte
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbInstall)
                ptrns = myDitte.getDitte(myIdTransazione)
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
        Public Function getDitteByCodice(ByVal Codice As String) As ArrayList
            Dim myFactory As FactoryDAO
            Dim myIdTransazione As Integer
            Dim myDitte As DitteDAO
            Dim myDataSource As DataSource = Nothing
            Dim bCommit As Boolean = True
            Dim ptrns As New ArrayList


            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                myDitte = myFactory.getDitteByCodice
                myIdTransazione = myDataSource.beginTransaction(DataSource.TipoConnessione.dbInstall)
                ptrns = myDitte.getDitteByCodice(myIdTransazione, Codice)
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
    End Class
End Namespace