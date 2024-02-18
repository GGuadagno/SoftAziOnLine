Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao
Imports It.SoftAzi.SystemFramework
Imports System.Text

Namespace It.SoftAzi.Model.Roles
    Public Class StringaConnessione
        Public Function getStringaConnessione(ByVal tipodb As It.SoftAzi.Integration.Dao.DataSource.TipoConnessione, Optional ByVal Anno As String = "") As String
            Dim myFactory As FactoryDAO
            Dim myDataSource As DataSource = Nothing
            Dim esito As String = ""
            Try
                myFactory = FactoryDAO.getFactoryDAO
                myDataSource = myFactory.getDataSource
                If Anno = "" Then
                    esito = myDataSource.getConnectionString(tipodb)
                Else
                    esito = myDataSource.getConnectionString(tipodb, Anno)
                End If
            Catch ex As Exception
                Throw ex
                'If log.IsErrorEnabled Then
                '    log.Error("Eccezione: " & ex.Message)
                'End If
            Finally
            End Try
            Return esito
        End Function

    End Class
End Namespace
