Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles
Imports It.SoftAzi.Integration.Dao
Namespace It.SoftAzi.Model.Facade

    Public Class dbStringaConnesioneFacade
        Dim myannoLogin As String = ""
        Public Function getConnectionString(ByVal TipoDb As It.SoftAzi.Integration.Dao.DataSource.TipoConnessione, Optional ByVal Anno As String = "") As String
            Dim mydb As New Roles.StringaConnessione
            If Anno = "" Then
                If myannoLogin = "" Then
                    Return mydb.getStringaConnessione(TipoDb)
                Else
                    Return mydb.getStringaConnessione(TipoDb, myannoLogin)
                End If

            Else
                Return mydb.getStringaConnessione(TipoDb, Anno)
            End If
        End Function

        Public Sub New(ByVal mioanno As String)
            myannoLogin = mioanno
        End Sub
    End Class
End Namespace