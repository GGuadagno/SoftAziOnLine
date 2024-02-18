Option Explicit On 

Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Configuration

Public Class clsDatabase

    Public Function getConnectionString(ByVal TipoDatabase As String) As String
        Dim x As String
        x = ConfigurationSettings.AppSettings.Get(TipoDatabase)
        x += ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
        Return x
    End Function

    Public Function SettaConnessione(ByRef objConn As SqlConnection, ByVal TipoDatabase As String) As Boolean

        Try
            objConn = New SqlConnection(Me.getConnectionString(TipoDatabase))
            Return True
        Catch
            objConn = Nothing
            MsgBox(Err.Number & ". " & Err.Description, MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, "Errore")
            Return False
        Finally

        End Try
    End Function

    Public Sub ControllaConnessione(ByRef objConnection As SqlConnection)
        If Not objConnection Is Nothing Then
            If objConnection.State = ConnectionState.Open Then
                objConnection.Close()
            End If
        End If
    End Sub

End Class
