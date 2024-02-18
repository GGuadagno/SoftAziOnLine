Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class TipoFatt
        Public Function getTipoFattByCodice(ByVal Codice As Integer) As ArrayList
            Dim myTipoFatt As New Roles.TipoFattRole
            Return myTipoFatt.getTipoFattByCodice(Codice)
        End Function
        Public Function getTipoFatt() As ArrayList
            Dim myTipoFatt As New Roles.TipoFattRole
            Return myTipoFatt.getTipoFatt
        End Function
    End Class
End Namespace
