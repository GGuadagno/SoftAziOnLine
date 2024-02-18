Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Zone
        Public Function getZone() As ArrayList
            Dim myZone As New Roles.ZoneRole
            Return myZone.getZone()
        End Function
    End Class
End Namespace

